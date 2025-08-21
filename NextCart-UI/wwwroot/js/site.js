// Function to update the cart badge (GLOBAL)
window.updateCartBadge = function (count) {
    console.log('updateCartBadge called with count:', count);
    const cartBadge = document.getElementById('cartBadge');
    if (cartBadge) {
        cartBadge.textContent = count > 0 ? count : '';
        if (count > 0) {
            cartBadge.classList.add('active');
        } else {
            cartBadge.classList.remove('active');
        }
    } else {
        console.error('Cart badge element with ID "cartBadge" not found!');
    }
};

// --- Document Ready for Global JavaScript Logic ---
$(document).ready(function () {
    console.log('site.js document ready and loaded.');

    const initialCartCountElement = document.getElementById('initialCartCount');
    if (initialCartCountElement) {
        const initialCount = parseInt(initialCartCountElement.value || initialCartCountElement.textContent) || 0;
        window.updateCartBadge(initialCount);
        console.log('Initialized cart badge with count:', initialCount);
    } else {
        console.warn('Element with ID "initialCartCount" not found. Cart badge might not initialize correctly on page load.');
        window.updateCartBadge(0);
    }

    $('[data-bs-toggle="tooltip"]').tooltip();

    // --- CRITICAL ADDITION: Unbind previous handlers before binding ---
    // This ensures that if the $(document).ready block runs multiple times,
    // you don't end up with duplicate click handlers.
    $(document).off('click', '.add-to-cart-btn'); // <-- ADD THIS LINE

    // Event Delegation for "Add to Cart" button click handler
    $(document).on('click', '.add-to-cart-btn', function (e) {
        e.preventDefault();
        console.log('Add to Cart button clicked! (via delegation)'); // Keep this for debugging

        var productId = $(this).data('product-id');
        var quantity = 1;

        if ($(this).is(':disabled')) {
            alert('This product is out of stock.');
            return;
        }

        var $clickedButton = $(this);
        var $productCard = $clickedButton.closest('.product-card');
        var $stockText = $productCard.find('p.stock span'); // This selector might need adjustment based on your HTML structure.
                                                            // In the provided .cshtml, it's just <p class="card-text stock">Stock: @product.StockQuantity</p>
                                                            // so $productCard.find('p.stock') would get the paragraph.

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ productId: productId, quantity: quantity }),
            success: function (response) {
                console.log('AJAX Success Response:', response);
                if (response.success) {
                    // Removed: alert(response.message); <--- THIS LINE IS REMOVED
                    window.updateCartBadge(response.newCartCount);

                    if (response.currentProductStock !== undefined) {
                        // Update the stock text (assuming p.stock span is where stock is displayed)
                        // Note: Your .cshtml for product cards has <p class="card-text stock">Stock: @product.StockQuantity</p>
                        // so $stockText refers to the <p> element itself.
                        // You might need to adjust the selector if 'Stock:' is always present and you only want to update the number.
                        // For instance, if you changed it to <p class="card-text stock">Stock: <span id="stock-product-@product.productId">@product.StockQuantity</span></p>
                        // then you could use $stockText.text(response.currentProductStock);
                        // For now, I'll assume $stockText correctly targets the element showing the quantity.
                        $stockText.text('Stock: ' + response.currentProductStock);
                        $stockText.toggleClass('stock-in', response.currentProductStock > 0);
                        $stockText.toggleClass('stock-out', response.currentProductStock <= 0);

                        if (response.currentProductStock <= 0) {
                            $clickedButton.prop('disabled', true);
                            $clickedButton.html('<i class="fas fa-exclamation-circle"></i> <span>Out of Stock</span>');
                        } else {
                            $clickedButton.prop('disabled', false); // Enable if stock becomes available (e.g., if quantity was reduced from cart)
                            $clickedButton.html('<i class="fas fa-cart-plus"></i> <span>Add to Cart</span>');
                        }
                    }
                } else {
                    // Failure: Show a normal JavaScript alert (as requested)
                    alert('Failed to add product to cart: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error, xhr.responseText);
                // General error: Show a normal JavaScript alert (as requested)
                alert('An unexpected error occurred while adding to cart. Please try again.');
            }
        });
    });
});