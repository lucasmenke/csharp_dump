using Stripe;

namespace Ecommerce.Server.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    private readonly IOrderService _orderService;
    private string _secret;

    public PaymentService(IConfiguration configuration, ICartService cartService, IAuthService authService, IOrderService orderService)
    {
        _configuration = configuration;
        _cartService = cartService;
        _authService = authService;
        _orderService = orderService;

        StripeConfiguration.ApiKey = _configuration.GetSection("Stripe:API").Value;
        _secret = _configuration.GetSection("Stripe:Webhook").Value;

    }
    
    public async Task<Session> CreateCheckoutSession()
    {
        var products = (await _cartService.GetDbCartProducts()).Data;
        var lineItems = new List<SessionLineItemOptions>();
        products.ForEach(p => lineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmountDecimal = p.Price * 100,
                Currency = "eur",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = p.Title,
                    Images = new List<string> 
                    { 
                        p.ImageUrl 
                    }
                }
            },
            Quantity = p.Quantity
        }));

        var options = new SessionCreateOptions
        {
            CustomerEmail = _authService.GetUserEmail(),
            ShippingAddressCollection = new SessionShippingAddressCollectionOptions
            {
                AllowedCountries = new List<string>
                {
                    "DE"
                }
            },
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = _configuration.GetSection("Stripe:SuccessUrl").Value,
            CancelUrl = _configuration.GetSection("Stripe:CancelUrl").Value
        };

        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }

    public async Task<ServiceResponse<bool>> FullfillOrder(HttpRequest request)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        try 
        {
            var stripEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], _secret);

            if (stripEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripEvent.Data.Object as Session;
                var user = await _authService.GetUserByEmail(session.CustomerEmail);
                await _orderService.PlaceOrder(user.Id);
            }

            return new ServiceResponse<bool>
            {
                Data = true,
            };
        }
        catch (StripeException ex)
        {
            return new ServiceResponse<bool>
            {
                Data = false,
                Success = false,
                Message = ex.Message
            };
        }
    }
}
