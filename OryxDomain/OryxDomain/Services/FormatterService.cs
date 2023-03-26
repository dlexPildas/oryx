using OryxDomain.Repository;
using OryxDomain.Utilities;
using Microsoft.Extensions.Configuration;
using OryxDomain.Models.Oryx;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using OryxDomain.Models.Enums;

namespace OryxDomain.Services
{
    public class FormatterService
    {
        private readonly IConfiguration Configuration;
        private readonly DictionaryRepository DictionaryRepository;
        public FormatterService(IConfiguration configuration)
        {
            Configuration = configuration;
            DictionaryRepository = new DictionaryRepository(Configuration["OryxPath"] + "oryx.ini");
        }

        public async Task<string> GetNextNumber(string field)
        {
            Int64 nextNumber = await DictionaryRepository.GetLastNumber(field);
            int fieldSize = await FindFieldLength(field);
            nextNumber++;
            return Formatters.FormatId(nextNumber.ToString(), fieldSize);
        }

        public async Task<Object> ValidateFormatBasicByDC1(Object entityObject)
        {
            Type objType = entityObject.GetType();
            List<DC1> tableFields = await DictionaryRepository.FindDC1ByDc1arquivo(objType.Name) as List<DC1>;
            if (tableFields != null && tableFields.Count > 0)
            {
                PropertyInfo[] properties = objType.GetProperties();
                foreach (PropertyInfo propertie in properties)
                {
                    DC1 dc1 = tableFields.Find(dc1 => dc1.Dc1campo.ToUpper() == propertie.Name.ToUpper());
                    if (dc1 != null && dc1.Dc1edicao != (int)Dc1edicaoType.INVISIBLE)
                    {
                        var propertieValue = propertie.GetValue(entityObject, null);
                        if (propertie.PropertyType == typeof(string) && propertieValue == null)
                        {
                            propertie.SetValue(entityObject, string.Empty);
                            propertieValue = string.Empty;
                        }
                        if (propertie.PropertyType == typeof(DateTime?) && propertieValue == null)
                        {
                            propertieValue = Constants.MinDateOryx;
                        }
                        if ((propertieValue == null || propertieValue.Equals(string.Empty)) &&
                            dc1.Dc1nulo == 0 &&
                            dc1.Dc1edicao < (int)Dc1edicaoType.NOTEDITABLE)
                        {
                            throw new MissingFieldException("Informar campo " + dc1.Dc1nome + ".");
                        }
                        else if (propertieValue != null)
                        {
                            switch (dc1.Dc1tipo)
                            {
                                case "C":
                                    if (((string)propertieValue).Length > 0 &&
                                        string.IsNullOrWhiteSpace(((string)propertieValue).Trim()) &&
                                        dc1.Dc1espaco == 0)
                                    {
                                        throw new MissingFieldException("Campo " + dc1.Dc1nome + " não aceita espaço em branco.");
                                    }
                                    //if (dc1.Dc1formato == DictionaryFormatType.PADLEFTZEROS && !string.IsNullOrWhiteSpace(((string)propertieValue).Trim()))
                                    //    propertieValue = Formatters.FormatId(propertieValue.ToString(), dc1.Dc1tamanho);
                                    
                                    propertieValue = Formatters.FormatField(propertieValue.ToString(), dc1.Dc1tamanho);
                                    break;
                                case "N":

                                    break;
                                case "T":
                                case "D":
                                    break;
                            }
                        }
                        propertie.SetValue(entityObject, propertieValue);

                    }
                }
            }
            return entityObject;
        }

        public async Task<int> FindFieldLength(string column)
        {
            int? size = await DictionaryRepository.FindFieldLengthOryx(column);

            if (!size.HasValue)
            {
                throw new Exception(string.Format(Resources.Resources.Message_DictionaryColumnNotFound, column));
            }
            return size.Value;
        }
    }
}
