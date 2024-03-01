using Mapster;
using Product.Data.Dtos;
using ProductData.MongoCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Product.Data.Configurations
{
    public class MapperConfig
    { 
        public static void Configure()
        {
            TypeAdapterConfig<ProductDto, Products>.NewConfig()
                .Map(dest => dest.Price.GiaTienGoc, src => src.GiaTienGoc)
                .Map(dest => dest.Price.GiaTienHienTai, src => src.GiaTienHienTai)
                .Map(dest => dest.Price.PhanTramGiamGia, src => src.PhanTramGiamGia);
        }
    }
}
