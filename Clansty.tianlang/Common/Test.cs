﻿using System;
using System.Data;
using Newtonsoft.Json;
using SpreadsheetLight;

namespace Clansty.tianlang
{
    public static class Test
    {
        public static void Do()
        {
            C.WriteLn(JsonConvert.SerializeObject(Q2TgMap.infos));
        }

        static DataTable XlsxToDataTable(string vFilePath)
        {
            var dataTable = new DataTable();
            try
            {
                var sldocument = new SLDocument(vFilePath);
                dataTable.TableName = sldocument.GetSheetNames()[0];
                var worksheetStatistics = sldocument.GetWorksheetStatistics();
                var startColumnIndex = worksheetStatistics.StartColumnIndex;
                var endColumnIndex = worksheetStatistics.EndColumnIndex;
                var startRowIndex = worksheetStatistics.StartRowIndex;
                var endRowIndex = worksheetStatistics.EndRowIndex;
                for (var i = startColumnIndex; i <= endColumnIndex; i++)
                {
                    var cellValueAsRstType = sldocument.GetCellValueAsRstType(1, i);
                    dataTable.Columns.Add(new DataColumn(cellValueAsRstType.GetText(), typeof(string)));
                }

                for (var j = startRowIndex + 1; j <= endRowIndex; j++)
                {
                    var dataRow = dataTable.NewRow();
                    for (var i = startColumnIndex; i <= endColumnIndex; i++)
                    {
                        dataRow[i - 1] = sldocument.GetCellValueAsString(j, i);
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Xlsx to DataTable: \n" + ex.Message);
            }

            return dataTable;
        }
    }
}