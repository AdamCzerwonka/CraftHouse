const inputs = [...document.querySelectorAll(".product__option")];
const checkboxes = [...document.querySelectorAll("input[type=checkbox]")];
const productPriceElement = document.querySelector(".total__price__value");

let price = parseFloat(document.querySelector(".product__price").getAttribute("data-price"));

inputs.forEach((input) => {
    input.addEventListener("click", () => {

        let isChecked = input.getAttribute("data-checked") === "true";
        let id = input.getAttribute("data-id");
        let optionId = input.getAttribute("data-option-id");
        let maxOccurs = input.getAttribute("data-max-occurs");

        let checkedValues = inputs
            .filter(input => input.getAttribute("data-option-id") === optionId)
            .filter(input => input.getAttribute("data-checked") === "true")
            .length


        if (!isChecked) {
            if (checkedValues === +maxOccurs) {
                return
            }
        }

        inputs
            .filter(input => input.getAttribute("data-option-id") === optionId)
            .forEach(input => input.classList.remove("disabled"))

        let checkBox = checkboxes.find((x) => x.value === id);
        checkBox.checked = !checkBox.checked;
        input.setAttribute("data-checked", checkBox.checked);
        input.classList.toggle("checked");
        let child = input.querySelector("img");
        child.classList.toggle("hidden");

        let inputPrice = parseFloat(input.getAttribute("data-price"))
        if (!isChecked) {
            price += inputPrice;
        } else {
            price -= inputPrice;
        }
        
        updateTotalPrice();
        

        if (!isChecked) {
            if (checkedValues + 1 === +maxOccurs) {
                inputs
                    .filter(input => input.getAttribute("data-option-id") === optionId)
                    .filter(input => input.getAttribute("data-checked") === "false")
                    .forEach(input => input.classList.add("disabled"))
            }
        }
    });
});

document.getElementById("addToCartButton").addEventListener("click", () => {
    document.getElementById("addToCartForm").submit();
})

document.querySelector("form > input[type=submit]").style.display = "none";
checkboxes.forEach(checkbox => checkbox.style.display = "none");

function updateTotalPrice() {
   const totalPrice = document.querySelector(".total__price__value"); 
   totalPrice.textContent = `$${price.toFixed(2)}`
}