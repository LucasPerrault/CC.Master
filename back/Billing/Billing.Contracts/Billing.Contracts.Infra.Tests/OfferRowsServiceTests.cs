using Billing.Contracts.Domain.Offers.Parsing;
using Billing.Contracts.Infra.Offers;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace Billing.Contracts.Infra.Tests
{
    public class OfferRowsServiceTests
    {
        [Fact]
        public async Task ShouldUpload()
        {
            var s = new StringBuilder();
            s.AppendLine("Nom,Id produit,Unite de decompte,Id devise,Categorisation,Mode Decompte,Methode de pricing,Algorithme previsionnel,Date de debut de la grille,Borne inferieure,Borne superieure,Prix unitaire,Prix forfaitaire");
            s.AppendLine("Cleemy template 2021,2,1,978,catalogues,3,Linear,LastRealMonth,15/12/2021,0,10,0,50");
            s.AppendLine(",,,,,,,,,11,20,2,0");
            s.AppendLine(",,,,,,,,,21,50,1.9,0");
            s.AppendLine(",,,,,,,,,51,100,1.5,0");
            s.AppendLine(",,,,,,,,,101,1000,1,0");
            s.AppendLine(",,,,,,,,01/08/2020,0,10,0,30");
            s.AppendLine(",,,,,,,,,11,20,1.5,0");
            s.AppendLine(",,,,,,,,,21,50,1.4,0");
            s.AppendLine(",,,,,,,,,51,100,1,0");
            var myByteArray = Encoding.UTF8.GetBytes(s.ToString());
            var ms = new MemoryStream(myByteArray);

            using var reader = new StringReader(s.ToString());
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());


            var offers = await sut.UploadAsync(ms);

            offers.Should().HaveCount(1);
            offers.First().PriceLists.Should().HaveCount(2);
            offers.First().PriceLists.First().Rows.Should().HaveCount(5);
            offers.First().PriceLists.Last().Rows.Should().HaveCount(4);
        }

        [Fact]
        public void ShouldThrowWhenUploadWithBadHeader()
        {
            var s = new StringBuilder();
            s.AppendLine("Nom,Id produit,Unite de decomd,Id devise,Categorisation,Mode Decompte,Methode de pricing,Algorithme previsionnel,Date de debut de la grille,Borne inferieure,Borne superieure,Prix unitaire,Prix forfaitaire");
            s.AppendLine("Cleemy template 2021,2,1,978,catalogues,3,Linear,LastRealMonth,15/12/2021,0,10,0,50");
            s.AppendLine(",,,,,,,,,11,20,2,0");
            s.AppendLine(",,,,,,,,,21,50,1.9,0");
            s.AppendLine(",,,,,,,,,51,100,1.5,0");
            s.AppendLine(",,,,,,,,,101,1000,1,0");
            s.AppendLine(",,,,,,,,01/08/2020,0,10,0,30");
            s.AppendLine(",,,,,,,,,11,20,1.5,0");
            s.AppendLine(",,,,,,,,,21,50,1.4,0");
            s.AppendLine(",,,,,,,,,51,100,1,0");
            var myByteArray = Encoding.UTF8.GetBytes(s.ToString());
            var ms = new MemoryStream(myByteArray);

            using var reader = new StringReader(s.ToString());
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());

            Func<Task<List<ParsedOffer>>> func = () => sut.UploadAsync(ms);

            func.Should().Throw<MissingFieldException>().WithMessage("*unité de décompte*");
        }

        [Fact]
        public async Task ShouldUploadTemplate()
        {
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());

            var template = await sut.GetTemplateStreamAsync();

            var offers = await sut.UploadAsync(template);

            offers.Should().HaveCount(2);
            offers.First().PriceLists.Should().HaveCount(2);
            offers.First().PriceLists.First().Rows.Should().HaveCount(5);
            offers.First().PriceLists.Last().Rows.Should().HaveCount(5);
        }

        [Fact]
        public async Task ShouldThrowWhenUploadTemplateWithBadHeader()
        {
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());

            var template = await sut.GetTemplateStreamAsync();
            using var reader = new StreamReader(template);
            var stringTemplate = reader.ReadToEnd();

            stringTemplate = stringTemplate.Replace("unite de decompte", "unite de decompd", StringComparison.CurrentCultureIgnoreCase);
            Func<Task<List<ParsedOffer>>> func = () => sut.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(stringTemplate)));

            func.Should().Throw<MissingFieldException>().WithMessage("*unité de décompte*");
        }
    }
}
