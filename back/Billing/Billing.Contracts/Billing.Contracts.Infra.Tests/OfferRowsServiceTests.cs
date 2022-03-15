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
using Lucca.Core.Shared.Domain.Exceptions;
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
            s.AppendLine($"{HeaderRow.Name},{HeaderRow.Product},{HeaderRow.BillingUnit},{HeaderRow.Currency},{HeaderRow.Tag},{HeaderRow.BillingMode},{HeaderRow.PricingMethod},{HeaderRow.ForecastMethod},{HeaderRow.ListStartsOn},{HeaderRow.RowMin},{HeaderRow.RowMax},{HeaderRow.RowUnitPrice},{HeaderRow.RowFixedPrice}");
            s.AppendLine("Cleemy template 2021,2,1,EUR,catalogues,3,Linear,LastRealMonth,15/12/2021,0,10,0,50");
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
        public async Task ShouldThrowWhenUploadWithBadHeader()
        {
            var s = new StringBuilder();
            s.AppendLine($"{HeaderRow.Name},{HeaderRow.Product},toto,{HeaderRow.Currency},{HeaderRow.Tag},{HeaderRow.BillingMode},{HeaderRow.PricingMethod},{HeaderRow.ForecastMethod},{HeaderRow.ListStartsOn},{HeaderRow.RowMin},{HeaderRow.RowMax},{HeaderRow.RowUnitPrice},{HeaderRow.RowFixedPrice}");
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

            await func.Should().ThrowAsync<MissingFieldException>().WithMessage("*unité de décompte*");
        }

        [Fact]
        public async Task ShouldThrowWhenUploadWithBadDelimiter()
        {
            var s = new StringBuilder();
            var badDelimiter = ";";
            s.AppendLine($"{HeaderRow.Name}{badDelimiter}{HeaderRow.Product}{badDelimiter}toto{badDelimiter}{HeaderRow.Currency}{badDelimiter}{HeaderRow.Tag}{badDelimiter}{HeaderRow.BillingMode}{badDelimiter}{HeaderRow.PricingMethod}{badDelimiter}{HeaderRow.ForecastMethod}{badDelimiter}{HeaderRow.ListStartsOn}{badDelimiter}{HeaderRow.RowMin}{badDelimiter}{HeaderRow.RowMax}{badDelimiter}{HeaderRow.RowUnitPrice}{badDelimiter}{HeaderRow.RowFixedPrice}");
            s.AppendLine($"Cleemy template 2021{ badDelimiter }2{ badDelimiter }1{ badDelimiter }978{ badDelimiter }catalogues{ badDelimiter }3{ badDelimiter }Linear{ badDelimiter }LastRealMonth{ badDelimiter }15/12/2021{ badDelimiter }0{ badDelimiter }10{ badDelimiter }0{ badDelimiter }50");
            var myByteArray = Encoding.UTF8.GetBytes(s.ToString());
            var ms = new MemoryStream(myByteArray);

            using var reader = new StringReader(s.ToString());
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());

            Func<Task<List<ParsedOffer>>> func = () => sut.UploadAsync(ms);

            await func.Should().ThrowAsync<BadRequestException>().WithMessage("*Csv delimiter must be*");
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

            stringTemplate = stringTemplate.Replace(HeaderRow.BillingUnit, "unite de decompd", StringComparison.CurrentCultureIgnoreCase);
            Func<Task<List<ParsedOffer>>> func = () => sut.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(stringTemplate)));

            await func.Should().ThrowAsync<MissingFieldException>().WithMessage("*unité de décompte*");
        }

        [Fact]
        public async Task ShouldThrowWhenUploadTemplateWithBadDelimiter()
        {
            var productsStoreMock = new Mock<IProductsStore>();
            productsStoreMock.Setup(x => x.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>())).ReturnsAsync(new List<Product>());

            var sut = new OfferRowsService(productsStoreMock.Object, new ParsedOffersService());

            var template = await sut.GetTemplateStreamAsync();
            using var reader = new StreamReader(template);
            var stringTemplate = reader.ReadToEnd();

            var badDelimiter = ";";
            stringTemplate = stringTemplate.Replace(",", badDelimiter, StringComparison.CurrentCultureIgnoreCase);
            Func<Task<List<ParsedOffer>>> func = () => sut.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(stringTemplate)));

            await func.Should().ThrowAsync<BadRequestException>().WithMessage("*Csv delimiter must be*");
        }
    }
}
