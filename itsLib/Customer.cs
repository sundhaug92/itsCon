﻿using System;
using HtmlAgilityPack;

namespace itsLib
{
    public class Customer
    {
        private uint _Id;
        private string _Name;

        public Customer(Session Session, uint Id)
        {
            HtmlDocument Doc = Session.GetDocument("/");
            _Id = Id;
            foreach (var v in Doc.DocumentNode.Descendants("option"))
            {
                if (v.ParentNode.GetAttributeValue("id", "") != "ctl00_ContentPlaceHolder1_LoginSection1_ChooseSite_site_input") continue;
                if (v.GetAttributeValue("value", -1) == Id) _Name = v.NextSibling.InnerHtml;
            }
            if ((_Name == null) || (_Name == "")) throw new ArgumentException("No customer found", "Id");
        }

        public Customer(Session Session, string Name)
        {
            _Name = Name;
            HtmlDocument Doc = Session.GetDocument("/");
            foreach (var v in Doc.DocumentNode.Descendants("option"))
            {
                if (v.ParentNode.GetAttributeValue("id", "") != "ctl00_ContentPlaceHolder1_LoginSection1_ChooseSite_site_input") continue;
                if (_Name == v.NextSibling.InnerHtml) _Id = (uint)v.GetAttributeValue("value", 0);
            }
            if ((_Name == null) || (_Name == "")) throw new ArgumentException("No customer found", "Id");
        }

        public uint Id
        {
            get
            {
                return _Id;
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }
    }
}