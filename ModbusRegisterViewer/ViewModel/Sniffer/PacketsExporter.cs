using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer.ViewModel.Sniffer
{
    public static class PacketsExporter
    {
        public static void Export(string filename, ObservableCollection<PacketViewModel> packets)
        {
            using(var writer = File.CreateText(filename))
            {
                writer.WriteLine("Time,Milliseconds,Address,Func,Description,Direction,Interval,CRC,Size");

                foreach(var packet in packets)
                {
                    writer.WriteLine("{0},{1},{2},\"{3}\",\"{4}\",{5},{6},{7}",
                        packet.Time,
                        packet.Address,
                        packet.Function,
                        packet.Type,
                        packet.Direction,
                        packet.ResponseTime,
                        packet.CRC,
                        packet.Bytes);
                }

            }


        }

        //public static void Export(string filename, ObservableCollection<PacketViewModel> packets)
        //{
        //    using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filename, SpreadsheetDocumentType.Workbook))
        //    {
        //        //Create the workbook part
        //        var workbookPart = spreadsheetDocument.AddWorkbookPart();
        //        workbookPart.Workbook = new Workbook();

        //        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //        worksheetPart.Worksheet = new Worksheet(new SheetData());

        //        //Add sheets to the workbook
        //        var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

        //        var sheet = new Sheet()
        //        {
        //            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
        //            SheetId = 1,
        //            Name = "Modbus Capture"
        //        };

        //        //Append it
        //        sheets.Append(sheet);

        //        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


        //        var columnNames = new string[] {
        //                "Time",
        //                "Milliseconds",
        //                "Address",
        //                "Func",
        //                "Description",
        //                "Direction",
        //                "Interval",
        //                "CRC",
        //                "Size"
        //            };

        //        //Add the column names
        //        sheetData.AddHeaderRow(columnNames);

        //        for (int packetIndex = 0; packetIndex < packets.Count; packetIndex++)
        //        {
        //            var packet = packets[packetIndex];

        //            var dataRow = new Row()
        //            {
        //                RowIndex = (UInt32)(packetIndex + 2)
        //            };

        //            //Time
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("A{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Time),
        //                DataType = new EnumValue<CellValues>(CellValues.String)
        //            }, 0);

        //            //Millseconds
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("B{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Milliseconds.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 1);

        //            //Address
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("C{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Address.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 2);

        //            //Func
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("D{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Function.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 3);

        //            //Description
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("E{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Type),
        //                DataType = new EnumValue<CellValues>(CellValues.String)
        //            }, 4);

        //            //Direction
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("F{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Direction.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.String)
        //            }, 5);

        //            //Interval
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("G{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.ResponseTime.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 6);

        //            //CRC
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("H{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.CRC.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 7);

        //            //Size
        //            dataRow.InsertAt(new Cell()
        //            {
        //                CellReference = string.Format("I{0}", dataRow.RowIndex),
        //                CellValue = new CellValue(packet.Bytes.ToString()),
        //                DataType = new EnumValue<CellValues>(CellValues.Number)
        //            }, 8);

        //            sheetData.Append(dataRow);
        //        }

        //        //Save it
        //        workbookPart.Workbook.Save();
        //    }
        //}
    }
}
