using System;
using System.Collections.Generic;
using System.Text;

// HotelManagement.BLL/DTOs/PhongTrongDto.cs
namespace HotelManagement.BLL.DTOs
{
    public class Phongtrongdto
    {
        public int IdPhong { get; set; }
        public string SoPhong { get; set; } = null!;
        public int IdLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; } = null!;
        public decimal Gia { get; set; }
        public int SucChua { get; set; }
    }
}
