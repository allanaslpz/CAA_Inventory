// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function generateSKU()
{
    // Generate random SKU generation that start with CAA and between 0 and 999999
    var sku = "CAA" + Math.floor(Math.random() * 1000000);
    document.getElementById("SKUNumber").value = sku;    
}
