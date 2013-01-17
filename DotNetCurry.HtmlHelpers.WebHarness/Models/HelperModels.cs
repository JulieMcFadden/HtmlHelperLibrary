using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetCurry.HtmlHelpers.WebHarness.Models
{
    public class TwoDimensionalData
    {
        public int Id { get; set; }
        private List<string[]> _data;

        public List<string[]> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public TwoDimensionalData()
        {
            _data = new List<string[]>();

        }

    }
}