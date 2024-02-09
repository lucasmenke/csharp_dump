using Blazored.LocalStorage;
using Ecommerce.Shared;

namespace Ecommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private readonly IAuthService _authService;

    public CartService(ILocalStorageService localStorage, HttpClient http, IAuthService authService)
    {
        _localStorage = localStorage;
        _http = http;
        _authService = authService;
    }

    public event Action OnChange;

    public async Task AddToCart(CartItem cartItem)
    {
        if (await _authService.IsUserAuthenticated())
        {
            await _http.PostAsJsonAsync("api/cart/add", cartItem);
        }
        else
        {
            List<CartItem>? cart = await GetCartFromLocalStorage();

            var sameItem = cart.Find(p => p.ProductId == cartItem.ProductId && p.ProductTypeId == cartItem.ProductTypeId);
            if (sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);
        }

        await GetCartItemsCount();
    }

    private async Task<List<CartItem>> GetCartFromLocalStorage()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            cart = new();
        }

        return cart;
    }
    public async Task<List<CartProductResponse>> GetCartProducts()
    {
        if (await _authService.IsUserAuthenticated())
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<CartProductResponse>>>("api/cart");
            return response.Data;
        }
        else
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cartItems == null)
            {
                return new List<CartProductResponse>();
            }
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
            return cartProducts.Data;
        }
    }

    public async Task RemoveProductFromCard(int productId, int productTypeId)
    {
        if (await _authService.IsUserAuthenticated())
        {
            await _http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
        }
        else
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cartItems == null)
            {
                return;
            }

            var cartItem = cartItems.Find(p => p.ProductId == productId && p.ProductTypeId == productTypeId);
            if (cartItem != null)
            {
                cartItems.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cartItems);
            }
        }
    }

    public async Task UpdateQuantity(CartProductResponse product)
    {
        if (await _authService.IsUserAuthenticated())
        {
            var request = new CartItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity
            };

            await _http.PutAsJsonAsync("api/cart/update-quantity", request);
        }
        else
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cartItems == null)
            {
                return;
            }

            var cartItem = cartItems.Find(p => p.ProductId == product.ProductId && p.ProductTypeId == product.ProductTypeId);
            if (cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorage.SetItemAsync("cart", cartItems);
            }
        }
    }

    public async Task StoreCartItems(bool emptyLocalCart = false)
    {
        var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (localCart == null)
        {
            return;
        }

        await _http.PostAsJsonAsync("api/cart", localCart);

        if (emptyLocalCart)
        {
            await _localStorage.RemoveItemAsync("cart");
        }
    }

    public async Task GetCartItemsCount()
    {
        if (await _authService.IsUserAuthenticated())
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
            var count = result.Data;

            await _localStorage.SetItemAsync<int>("cartItemsCount", count);
        }
        else
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            await _localStorage.SetItemAsync<int>("cartItemsCount", cart != null ? cart.Count : 0);
        }

        OnChange.Invoke();
    }
}
