using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Utility;
using Stripe;
using Stripe.Checkout;
using Moq;
using Xunit;
using ShoppingCart.Models;
using System.Linq.Expressions;

namespace ShoppingCart.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderVM OrderDetails(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(includeProperties: "Product").Where(x => x.OrderHeaderId == id)
            };

            return orderVM;
        }


        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public void SetToInProcess(OrderVM vm)
        {
            _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusInProcess);
            _unitOfWork.Save();
        }

        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public void SetToShipped(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            orderHeader.Carrier = vm.OrderHeader.Carrier;
            orderHeader.TrackingNumber = vm.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = OrderStatus.StatusShipped;
            orderHeader.DateOfShipping = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
        }

        [Authorize(Roles = WebSiteRole.Role_Admin + "," + WebSiteRole.Role_Employee)]
        public void SetToCancelOrder(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == vm.OrderHeader.Id);
            if (orderHeader.PaymentStatus == PaymentStatus.StatusApproved)
            {
                var refundOptions = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(refundOptions);
                _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusCancelled);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(vm.OrderHeader.Id, OrderStatus.StatusCancelled);
            }
            _unitOfWork.Save();
        }


    }

    //NewTestClass
    public class OrderControllerTests
    {
        [Fact]

        //First Test
        public void OrderDetails_ReturnsCorrectViewModel()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var orderHeader = new OrderHeader { Id = 1, ApplicationUser = new ApplicationUser() };
            var orderDetails = new List<OrderDetail> { new OrderDetail { OrderHeaderId = 1 } }; // Создаем хотя бы один OrderDetail с OrderHeaderId равным 1
            unitOfWorkMock.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>())).Returns(orderHeader);
            unitOfWorkMock.Setup(uow => uow.OrderDetail.GetAll(It.IsAny<string>())).Returns(orderDetails.AsQueryable());
            var controller = new OrderController(unitOfWorkMock.Object);

            // Act
            var result = controller.OrderDetails(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderHeader, result.OrderHeader);
            Assert.Equal(orderDetails, result.OrderDetails.ToList());
        }

        [Fact]
        //Second Test
        public void SetToInProcess_UpdatesOrderStatus()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var orderHeader = new OrderHeader { Id = 1 };
            var orderVM = new OrderVM { OrderHeader = orderHeader };
            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>(); // Создаем макет репозитория заказов
            var controller = new OrderController(unitOfWorkMock.Object);

            // Setup mock behavior
            unitOfWorkMock.SetupGet(uow => uow.OrderHeader).Returns(orderHeaderRepositoryMock.Object); // Установка поведения для доступа к репозиторию заказов

            // Act
            controller.SetToInProcess(orderVM);

            // Assert
            orderHeaderRepositoryMock.Verify(repo => repo.UpdateStatus(orderHeader.Id, OrderStatus.StatusInProcess, null), Times.Once); // Проверяем, что метод UpdateStatus был вызван один раз с правильными аргументами
            unitOfWorkMock.Verify(uow => uow.Save(), Times.Once); // Проверяем, что метод Save был вызван один раз
        }

        //Third Test

        [Fact]
        // Third Test
        public void SetToShipped_UpdatesOrderStatusToShipped()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();
            var controller = new OrderController(unitOfWorkMock.Object);

            // Создаем фиктивный объект OrderHeader
            var orderHeader = new OrderHeader { Id = 1, Carrier = "TestCarrier", TrackingNumber = "TestTrackingNumber", OrderStatus = OrderStatus.StatusShipped, DateOfShipping = DateTime.Now };

            // Настроим макет репозитория заказов, чтобы возвращать этот объект OrderHeader при вызове метода GetT
            unitOfWorkMock.Setup(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>())).Returns(orderHeader);
            // Act
            controller.SetToShipped(new OrderVM { OrderHeader = orderHeader });

            // Assert
            unitOfWorkMock.Verify(uow => uow.OrderHeader.GetT(It.IsAny<Expression<Func<OrderHeader, bool>>>(), null), Times.Once);
            unitOfWorkMock.Verify(uow => uow.OrderHeader.Update(It.IsAny<OrderHeader>()), Times.Once); // Проверяем, что метод Update был вызван один раз
            unitOfWorkMock.Verify(uow => uow.Save(), Times.Once); // Проверяем, что метод Save был вызван один раз
        }

    }

}
