using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ShouldReturnErrosWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(
                new FakeStudentRepository(), 
                new FakeEmailService());

            var command = new CreateBoletoSubscriptionCommand();
            command.FirstName = "Bruce";
            command.LastName = "Wayne";
            command.Document = "99999999999";
            command.Email = "hello@balta.io2";
            command.BarCode = "123456789";
            command.BoletoNumber = "123456";
            command.PaymentNumber = "123456";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "WAYNE CORP";
            command.PayerDocument = "12345678";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "batima@dc.com";
            command.Street = "street";
            command.Number = "1234";
            command.Neighborhood = "TAQUERA";
            command.City = "SP";
            command.State = "SP";
            command.Country = "BRAZIL";
            command.ZipCode = "08253180";

            handler.Handle(command);

            Assert.AreEqual(false, handler.Valid);
        }
    }
}