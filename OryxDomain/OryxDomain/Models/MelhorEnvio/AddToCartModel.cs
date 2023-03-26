﻿using System.Collections.Generic;

namespace OryxDomain.Models.MelhorEnvio
{
    public class AddToCartModel
    {
        public int service { get; set; }
        public int agency { get; set; }
        public From from { get; set; }
        public To to { get; set; }
        public List<Product> products { get; set; }
        public List<Volume> volumes { get; set; }
        public Options options { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string document { get; set; }
        public string company_document { get; set; }
        public string state_register { get; set; }
        public string address { get; set; }
        public string complement { get; set; }
        public string number { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string country_id { get; set; }
        public string postal_code { get; set; }
        public string note { get; set; }
    }

    public class Invoice
    {
        public string key { get; set; }
    }

    public class Options
    {
        public double insurance_value { get; set; }
        public bool receipt { get; set; }
        public bool own_hand { get; set; }
        public bool reverse { get; set; }
        public bool non_commercial { get; set; }
        public Invoice invoice { get; set; }
        public string platform { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public double unitary_value { get; set; }
    }

    public class Tag
    {
        public string tag { get; set; }
        public string url { get; set; }
    }

    public class To
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string document { get; set; }
        public string company_document { get; set; }
        public string state_register { get; set; }
        public string address { get; set; }
        public string complement { get; set; }
        public string number { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string state_abbr { get; set; }
        public string country_id { get; set; }
        public string postal_code { get; set; }
        public string note { get; set; }
    }

    public class Volume
    {
        public int height { get; set; }
        public int width { get; set; }
        public int length { get; set; }
        public double weight { get; set; }
    }
}
