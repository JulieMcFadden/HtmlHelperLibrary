using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace DotNetCurry.HtmlHelpers
{
    public static class ChartExtensions
    {
        public static HtmlString Chart(this HtmlHelper helper, string chartName, List<string[]> dataSource, string xTitle, string yTitle, string chartHeight, string chartWidth,
            string chartColor)
        {
            return SetupHtml(chartName, GetDataSourceFromIntArray(dataSource), xTitle, yTitle, chartHeight, chartWidth, chartColor);
        }

        private static string GetDataSourceFromIntArray(List<string[]> dataSource)
        {
            Json.Encode(dataSource);
            return "arrDataSource = " + Json.Encode(dataSource);
        }

        private static HtmlString SetupHtml(string chartName, string dataSource, string xTitle, string yTitle, string chartHeight, string chartWidth, string chartColor)
        {
            TagBuilder container = new TagBuilder("div");
            container.Attributes.Add("style", "border: solid 1px #F0F0F0;");
            TagBuilder canvas = new TagBuilder("canvas");
            canvas.Attributes.Add("id", chartName);
            canvas.Attributes.Add("height", chartHeight);
            canvas.Attributes.Add("width", chartWidth);
            canvas.SetInnerText(@"Your browser does not support HTML5 Canvas");

            TagBuilder script = SetupScript(chartName, dataSource, xTitle, yTitle, chartColor);

            TagBuilder noScript = new TagBuilder("noscript");
            noScript.InnerHtml = @"This chart is unavailable because Javascript is disabled on your computer.  Please enable JavaScript and refresh this page to see the chart in action.";
            container.InnerHtml = canvas.ToString() + script.ToString() + noScript.ToString();
            return new HtmlString(container.ToString());
        }

        private static TagBuilder SetupScript(string chartName, string dataSource, string xTitle, string yTitle, string chartColor)
        {
            TagBuilder script = new TagBuilder("script");
            script.Attributes.Add("type", "text/javascript");

            script.InnerHtml = @"
    <!--
        // chart data
        var chartColor = '" + chartColor + @"';
        var arrDataSource = new Array();
        " + dataSource + @"
        var canvas;
        var context;
        // chart properties
        var cWidth, cHeight, cMargin, cSpace;
        var cMarginSpace, cMarginHeight;
        // bar properties
        var bWidth, bMargin, totalBars, maxDataValue;
        var bWidthMargin;
        // bar animation
        var ctr, numctr, speed;
        // axis property
        var totLabelsOnYAxis;


        // barchart constructor
        function barChart(data) {
            if(data!=null)
            {
                arrDataSource = data;
            }
            canvas = document.getElementById('" + chartName + @"');
            if (canvas && canvas.getContext) {
                context = canvas.getContext('2d');
            }

            chartSettings();
            drawAxisLabelMarkers();
            drawChartWithAnimation();
        }

        // initialize the chart and bar values
        function chartSettings() {
            // chart properties
            cMargin = 25;
            cSpace = 60;
            cHeight = canvas.height - 2 * cMargin - cSpace;
            cWidth = canvas.width - 2 * cMargin - cSpace;
            cMarginSpace = cMargin + cSpace;
            cMarginHeight = cMargin + cHeight;
            // bar properties
            bMargin = 15;
            totalBars = parseInt(arrDataSource.length);
            bWidth = (cWidth / totalBars) - bMargin;


            // find maximum value to plot on chart
            maxDataValue = 0;
            for (var i = 0; i < totalBars; i++) {
                var barVal = parseInt(arrDataSource[i][1]);
                if (parseInt(barVal) > parseInt(maxDataValue))
                    maxDataValue = barVal;
            }

            totLabelsOnYAxis = 10;
            context.font = '10pt Verdana';

            // initialize Animation variables
            ctr = 0;
            numctr = 100;
            speed = 10;
        }

        // draw chart axis, labels and markers
        function drawAxisLabelMarkers() {
            context.lineWidth = '2.0';
            // draw y axis
            drawAxis(cMarginSpace, cMarginHeight, cMarginSpace, cMargin);
            // draw x axis
            drawAxis(cMarginSpace, cMarginHeight, cMarginSpace + cWidth, cMarginHeight);
            context.lineWidth = '1.0';
            drawMarkers();
        }

        // draw X and Y axis
        function drawAxis(x, y, X, Y) {
            context.beginPath();
            context.moveTo(x, y);
            context.lineTo(X, Y);
            context.closePath();
            context.stroke();
        }

        // draw chart markers on X and Y Axis
        function drawMarkers() {
            var numMarkers = parseInt(maxDataValue / totLabelsOnYAxis);
            context.textAlign = 'right';
            context.fillStyle = '#000'; ;

            // Y Axis
            for (var i = 0; i <= totLabelsOnYAxis; i++) {
                markerVal = i * numMarkers;
                markerValHt = i * numMarkers * cHeight;
                var xMarkers = cMarginSpace - 5;
                var yMarkers = cMarginHeight - (markerValHt / maxDataValue);
                context.fillText(markerVal, xMarkers, yMarkers, cSpace);
            }

            // X Axis
            context.textAlign = 'center';
            for (var i = 0; i < totalBars; i++) {
                name = arrDataSource[i][0];

                markerXPos = cMarginSpace + bMargin + (i * (bWidth + bMargin)) + (bWidth / 2);
                markerYPos = cMarginHeight + 20;
                context.fillText(name, markerXPos, markerYPos, bWidth);
            }

            context.save();

            // Add Y Axis title
            context.translate(cMargin + 10, cHeight / 2);
            context.rotate(Math.PI * -90 / 180);
            context.fillText('" + yTitle + @"', 0, 0);

            context.restore();

            // Add X Axis Title
            context.fillText('" + xTitle + @"', cMarginSpace + (cWidth / 2), cMarginHeight + 50);
        }

        function drawChartWithAnimation() {
            // Loop through the total bars and draw
            for (var i = 0; i < totalBars; i++) {
                bVal = parseInt(arrDataSource[i][1]);
                bHt = (bVal * cHeight / maxDataValue) / numctr * ctr;
                bX = cMarginSpace + (i * (bWidth + bMargin)) + bMargin;
                bY = cMarginHeight - bHt - 2;
                drawRectangle(bX, bY, bWidth, bHt, true);
            }

            // timeout runs and checks if bars have reached the desired height
            // if not, keep growing
            if (ctr < numctr) {
                ctr = ctr + 1;
                setTimeout(arguments.callee, speed);
            }
        }

        function drawRectangle(x, y, w, h, fill) {
            context.beginPath();
            context.rect(x, y, w, h);
            context.closePath();
            context.stroke();

            if (fill) {
                var gradient = context.createLinearGradient(0, 0, 0, 300);
                if (chartColor == 'blue' || chartColor == '')
                {
                    gradient.addColorStop(0, 'blue');
                    gradient.addColorStop(1, 'rgba(67,103,306,.15)');
                }
                 else if (chartColor == 'red')
                {
                    gradient.addColorStop(0, 'red');
                    gradient.addColorStop(1, 'rgba(306,103,67,.15)');
                }
                 else if (chartColor == 'green')
                {
                    gradient.addColorStop(0, 'green');
                    gradient.addColorStop(1, 'rgba(67,306,103,.15)');
                }
                else
                {
                    gradient.addColorStop(0, 'blue');
                    gradient.addColorStop(1, 'rgba(67,103,306,.15)');
                }
                context.fillStyle = gradient;
                context.strokeStyle = gradient;
                context.fill();
            }
        }
        -->
    ";
            return script;
        }
    }
}
