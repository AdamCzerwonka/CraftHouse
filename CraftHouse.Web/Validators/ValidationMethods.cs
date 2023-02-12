namespace CraftHouse.Web.Validators;

public class ValidationMethods
{
    public static bool IsPriceValid(float price)
    {
        var toCheck = price.ToString();
        if (toCheck.IndexOf(",") == -1) return true;
        var precision = toCheck.Length - toCheck.IndexOf(",") - 1;
        return precision is 2 or 1;
    }
}