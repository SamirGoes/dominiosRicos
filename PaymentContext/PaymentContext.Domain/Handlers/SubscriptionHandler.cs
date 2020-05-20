using System;
using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler :
    Notifiable,
    IHandler<CreateBoletoSubscriptionCommand>,
    IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _service;

        public SubscriptionHandler(IStudentRepository repository, IEmailService service)
        {
            _repository = repository;
            _service = service;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            // Fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura.");
            }

            // Verificar se documento ja está cadastrado
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "Este cpf ja está em uso");

            // Verificar se email ja está cadastrado
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "Este email ja está em uso");
            
            // Gerar VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            
            // Gerar entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode, 
                command.BoletoNumber, 
                command.PaidDate, 
                command.ExpireDate, 
                command.Total, 
                command.TotalPaid, 
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType), 
                address, 
                email);
            
            // Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Agrupar validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            if(Invalid)
                return new CommandResult(false, "Não foi possivel realizar sua assinatura");
                
            // Salvar informações
            _repository.CreateSubscription(student);

            // Enviar email de boas vindas
            _service.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");
            
            // Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso!");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            // Fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura.");
            }

            // Verificar se documento ja está cadastrado
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "Este cpf ja está em uso");

            // Verificar se email ja está cadastrado
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "Este email ja está em uso");
            
            // Gerar VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
            
            // Gerar entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(
                command.TransactionCode, 
                command.PaidDate, 
                command.ExpireDate, 
                command.Total, 
                command.TotalPaid, 
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType), 
                address, 
                email);
            
            // Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Agrupar validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            // Salvar informações
            _repository.CreateSubscription(student);

            // Enviar email de boas vindas
            _service.Send(student.Name.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");
            
            // Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso!");
        }
    }
}