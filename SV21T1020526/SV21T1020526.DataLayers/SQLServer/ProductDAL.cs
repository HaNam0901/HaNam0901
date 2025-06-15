using Azure;
using Dapper;
using SV21T1020526.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020526.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1;
                            else
                                begin
                                        insert into Products(
                                        ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price,Photo , IsSelling)
                                        values(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo ,@IsSelling);
                                        select SCOPE_IDENTITY()
                                end;
                            ";
                var parameters = new
                {
                    ProductName = data.ProductName,
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling,
                };
                // thực thi câu lệnh 
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(
                            ProductID, AttributeName, AttributeValue, DisplayOrder)
                            values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            select SCOPE_IDENTITY()
                              ;
                            ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder,
                };
                // thực thi câu lệnh 
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }
        public IList<ProductPhoto> ListPhotos()
        {
            List<ProductPhoto> data;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos";
                data = (connection.Query<ProductPhoto>(sql: sql, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            return data;
        }
        public int AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(
                            ProductID, Photo, Description, DisplayOrder, IsHidden)
                            values(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            select SCOPE_IDENTITY()
                              ;
                            ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                // thực thi câu lệnh 
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) 
                            from Products
                            where (@searchValue = N'' OR ProductName like @searchValue)
                                        and (@categoryID = 0 OR CategoryID = @CategoryID)
                                        and (@supplierID = 0 OR SupplierID = @supplierID)
                                        and (Price >= @minPrice)
                                        and (@maxPrice <=0 OR Price <= @maxPrice)";
                var parameters = new
                {
                    searchValue = searchValue,
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                    // bên trái là tham số sql: bên phải là giá trị
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where ProductID = @ProductID
                            delete from ProductAttributes where ProductID = @ProductID
                            delete from Products where ProductID = @ProductID";
                var parameters = new { ProductID = productID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new { ProductID = productID };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {   
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
	                            select 1
                            else
	                            select 0
                            ";
                var parameters = new { ProductID = productID };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }
        
        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {

            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
		                            select *, 
		                            ROW_NUMBER() over(order by ProductName) as RowNumber
		                            from Products
		                            where (@searchValue = N'' OR ProductName like @searchValue)
                                        and (@categoryID = 0 OR CategoryID = @CategoryID)
                                        and (@supplierID = 0 OR SupplierID = @supplierID)
                                        and (Price >= @minPrice)
                                        and (@maxPrice <=0 OR Price <= @maxPrice)
	                            )	as t
                            where (@pageSize = 0)
	                            or (RowNumber between (@page -1) * @pageSize + 1 and @page * @pageSize)
                            ";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue,
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                    // bên trái là tham số sql: bên phải là giá trị
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            IList<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = productID,
                    // bên trái là tham số sql: bên phải là giá trị
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID
                            ";
                var parameters = new
                {
                    ProductID = productID,
                    // bên trái là tham số sql: bên phải là giá trị
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                            begin 
                                update Products
                                set ProductName = @ProductName, ProductDescription = @ProductDescription, SupplierID = @SupplierID,
			                                CategoryID = @CategoryID, Unit = @Unit, Price = @Price, Photo = @Photo, IsSelling = @IsSelling
                                where ProductID = @ProductID
                            end;";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName,
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling,

                };
                // thực thi câu lệnh 
                result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes
                                set AttributeName = @AttributeName,  AttributeValue = @AttributeValue, DisplayOrder = @DisplayOrder
                                where AttributeID = @AttributeID
                            ";

                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    AttributeName = data.AttributeName,
                    DisplayOrder = data.DisplayOrder,
                    AttributeValue = data.AttributeValue

                };
                // thực thi câu lệnh 
                result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos
                                set Photo = @Photo,  Description = @Description, DisplayOrder = @DisplayOrder, IsHidden = @IsHidden
                                where PhotoID = @PhotoID
                            ";
            
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    Photo = data.Photo,
                    Description = data.Description,
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden

                };
                // thực thi câu lệnh 
                result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

		
		
		
	}
}
