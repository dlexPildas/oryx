using System;
using System.Text.RegularExpressions;

namespace OryxDomain.Utilities
{
    public static class Validators
    {
		public static bool IsCnpj(string cnpj)
		{
			int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
			int soma;
			int resto;
			string digito;
			string tempCnpj;
			cnpj = cnpj.Trim();
			cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
			if (cnpj.Length != 14)
				return false;
			tempCnpj = cnpj.Substring(0, 12);
			soma = 0;
			for (int i = 0; i < 12; i++)
				soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
			resto = (soma % 11);
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;
			digito = resto.ToString();
			tempCnpj = tempCnpj + digito;
			soma = 0;
			for (int i = 0; i < 13; i++)
				soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
			resto = (soma % 11);
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;
			digito = digito + resto.ToString();
			return cnpj.EndsWith(digito);
		}

		public static bool IsCpf(string cpf, bool validateSequences = false)
		{
			int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
			string tempCpf;
			string digito;
			int soma;
			int resto;
			cpf = cpf.Trim();
			cpf = cpf.Replace(".", "").Replace("-", "");
			if (cpf.Length != 11)
				return false;

            if (validateSequences)
            {
				if (cpf.Equals("00000000000") ||
					cpf.Equals("11111111111") ||
					cpf.Equals("22222222222") ||
					cpf.Equals("33333333333") ||
					cpf.Equals("44444444444") ||
					cpf.Equals("55555555555") ||
					cpf.Equals("66666666666") ||
					cpf.Equals("77777777777") ||
					cpf.Equals("88888888888") ||
					cpf.Equals("99999999999"))
				{
					return false;
				}
            }

			tempCpf = cpf.Substring(0, 9);
			soma = 0;

			for (int i = 0; i < 9; i++)
				soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
			resto = soma % 11;
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;
			digito = resto.ToString();
			tempCpf = tempCpf + digito;
			soma = 0;
			for (int i = 0; i < 10; i++)
				soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
			resto = soma % 11;
			if (resto < 2)
				resto = 0;
			else
				resto = 11 - resto;
			digito = digito + resto.ToString();
			return cpf.EndsWith(digito);
		}

		public static bool IsPhone(string phone)
        {
			if (string.IsNullOrWhiteSpace(phone))
				return true;
			Regex rg = new Regex(@"(\(?\d{2}\)?\s)?(\d{4,5}\-\d{4})");
			return rg.IsMatch(phone);
        }

		public static bool IsEMail(string email)
        {
			Regex rg = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,7}|[0-9]{1,3})$");
			return rg.IsMatch(email);
		}

        public static bool IsCEP(string cep)
        {
			return new Regex(@"^\d{5}-\d{3}$").IsMatch(cep) || new Regex(@"^\d{8}$").IsMatch(cep);
        }

		public static void ValidatePass(string cf1senha)
		{
			int lengthPass = cf1senha.Length;
			if (lengthPass < 8)
				throw new Exception(Resources.Resources.Message_PassSizeValidation);
			if (lengthPass == Regex.Replace(cf1senha, "[A-Z]", "").Length)
				throw new Exception(Resources.Resources.Message_PassUpperCaseValidation);
			if (lengthPass == Regex.Replace(cf1senha, "[a-z]", "").Length)
				throw new Exception(Resources.Resources.Message_PassLowerCaseValidation);
			if (lengthPass == Regex.Replace(cf1senha, "[0-9]", "").Length)
				throw new Exception(Resources.Resources.Message_PassNumberValidation);
			//if (lengthPass == Regex.Replace(cf1senha, "[!@#$&*]", "").Length)
			//	throw new Exception(Resources.Message_EspecialCaracterValidation);
		}

        public static bool ComparePass(string cf1senha1, string cf1senha2)
        {
			return BCrypt.Net.BCrypt.Verify(cf1senha1, cf1senha2, true);
		}

		public static void ValidateCep(string cep)
		{
			if (string.IsNullOrWhiteSpace(cep))
			{
				throw new MissingFieldException("CEP não informado.");
			}
			if (!Validators.IsCEP(cep))
			{
				throw new MissingFieldException("CEP inválido.");
			}
		}

		public static bool OnlyNumbers(string value)
		{
			return new Regex(@"[^\d]").Match(value).Success;
		}
	}
}
