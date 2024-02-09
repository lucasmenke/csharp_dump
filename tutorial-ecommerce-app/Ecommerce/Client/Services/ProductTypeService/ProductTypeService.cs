namespace Ecommerce.Client.Services.ProductTypeService;

public class ProductTypeService : IProductTypeService
{
    private readonly HttpClient _http;

    public List<ProductType> ProductTypes { get; set; } = new();

    public event Action OnChange;

    public ProductTypeService(HttpClient http)
    {
        _http = http;
    }

    public async Task GetProductTypes()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/producttype");
        ProductTypes = result.Data;
    }

    public async Task AddProductType(ProductType productType)
    {
        var response = await _http.PostAsJsonAsync("api/producttype", productType);
        ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
        OnChange.Invoke();
    }

    public async Task UpdateProductType(ProductType productType)
    {
        var response = await _http.PutAsJsonAsync("api/producttype", productType);
        ProductTypes = (await response.Content.ReadFromJsonAsync<ServiceResponse<List<ProductType>>>()).Data;
        OnChange.Invoke();
    }

    public ProductType CreateNewProductType()
    {
        var newProductType = new ProductType
        {
            IsNew = true,
            Editing = true,
        };

        ProductTypes.Add(newProductType);
        OnChange.Invoke();
        return newProductType;
    }
}
