using System.ComponentModel;

namespace OryxDomain.Models.Enums
{
    public enum DictionaryFormatType
    {
        [Description("Nenhuma definição")]
        NONE = 0,
        [Description("Acrescenta zeros a esquerda numa sequencia numérica recebida como caracter")]
        PADLEFTZEROS = 1,
        [Description("Data ocultando hora")]
        DATE = 2,
        [Description("Imagem paisagem")]
        LANDSCAPEIMAGE = 3,
        [Description("Imagem retrato")]
        PORTRAITIMAGE = 4,
        [Description("Data e hora no mesmo campo")]
        DATETIME = 5,
    }
}
