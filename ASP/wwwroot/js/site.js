// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const elem = document.getElementById('message');
const alert = document.getElementById('alert');
document.addEventListener('submit', e => {
    const form = e.target;
    if(form.id == "auth-modal-form") {
        e.preventDefault();
        const login = form.querySelector('[name="AuthLogin"]').value;
        const password = form.querySelector('[name="AuthPassword"]').value;
        if (login.length == 0)
        {
            elem.innerHTML = "login field can't be blank";
        }
        else if (password.length == 0)
        {
            elem.innerHTML = "password filed can't be blank";
        }
        if (login.length > 0 && password.length > 0) 
        {
            const credentials = btoa(login + ':' + password);
            fetch("/User/Signin", {
                method: 'GET',
                headers: {
                    'Authorization': 'Basic ' + credentials
                }
            }).then(r => r.json())
                .then(j => {
                    if (j.status == 200) {
                        window.location.reload();
                    } else {
                        console.log(j);
                        elem.innerHTML = j.message;
                    }
                })
            console.log("Submission stopped");
        }
    }
    if (form.id == "admin-category-form")
    {
        e.preventDefault();
        const name = document.querySelector('[name="category-name"]').value;
        const description = document.querySelector('[name="category-description"]').value;
        const slug = document.querySelector('[name="category-slug"]').value;
        const image = document.querySelector('[name="category-image"]').value;
        if (name.length == 0 || description.length == 0 || slug.length == 0 || image == null)
        {
            alert.innerHTML = "There's a problem with input. Please check data";
            alert.style.visibility = 'visible';
        }
        if (name.length > 0 && description.length > 0 && slug.length > 0 && image) {
            if (alert.style.visibility == 'visible')
            {
                alert.style.visibility = 'hidden';
            }
            fetch("/Admin/AddCategory", {
                method: 'POST',
                body: new FormData(form)
            }).then(r => r.json())
                .then(j => {
                    if (j.status == 401) {
                        console.log(j);
                        alert.innerHTML = j.message;
                        alert.style.visibility = 'visible';
                    } else {
                        window.location.reload();
                    }
                });
        }
    }
    if (form.id == "admin-product-form")
    {
        e.preventDefault();
        const name = document.querySelector('[name="product-name"]').value;
        const images = document.querySelector('[name="product-image"]').value;
        const price = parseFloat(document.querySelector('[name = "product-price"]').value);
        const stock = parseInt(document.querySelector('[name = "product-stock"]').value);
        if (name.length == 0  || !images || price <= 0 || stock < 0)
        {
            alert.innerHTML = "There's a problem with input. Please check data";
            alert.style.visibility = 'visible';
        }
        if (name.length > 0 && images && price > 0 && stock >= 0) {
            if (alert.style.visibility == 'visible') {
                alert.style.visibility = 'hidden';
            }
            fetch("/Admin/AddProduct", {
                method: 'POST',
                body: new FormData(form)
            }).then(r => r.json())
                .then(j => {
                    if (j.status == 401) {
                        console.log(j);
                        alert.innerHTML = j.message;
                        alert.style.visibility = 'visible';
                    } else {
                        window.location.reload();
                    }
                });
        }
    }
})

document.addEventListener('DOMContentLoaded', e => {
    for (let fab of document.querySelectorAll('[data-cart-product-id]')){
        fab.addEventListener('click', addToCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-decrement]')){
        btn.addEventListener('click', decCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-increment]')){
        btn.addEventListener('click', incCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-delete]')){
        btn.addEventListener('click', deleteCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-cancel]')) {
        btn.addEventListener('click', cancelCartClick);
    }
    for (let btn of document.querySelectorAll('[data-cart-submit]')) {
        btn.addEventListener('click', submitCartClick);
    }
});

function incCartClick(e) {
    const cartId = e.target.closest('[data-cart-increment]').getAttribute('data-cart-increment');
    console.log("++", cartId);
    modifyCartItem(cartId, 1)
}

function decCartClick(e) {
    const cartId = e.target.closest('[data-cart-decrement]').getAttribute('data-cart-decrement');
    console.log("--", cartId);
    modifyCartItem(cartId, -1)
}

function deleteCartClick(e) {
    const cartId = e.target.closest('[data-cart-delete]').getAttribute('data-cart-delete');
    const q = e.target.closest('.cart-item-row').querySelector('[data-cart-quantity]').innerText;
    console.log("xx", cartId);
    modifyCartItem(cartId, -q);
}

function cancelCartClick(e) {
    const cartId = e.target.closest('[data-cart-cancel]').getAttribute('data-cart-cancel');
    const c = 0;
    console.log("cart canceled", cartId);
    cartHandler(cartId, c);
}

function submitCartClick(e) {
    const cartId = e.target.closest('[data-cart-submit]').getAttribute('data-cart-submit');
    const c = 1;
    console.log("cart submitted", cartId);
    cartHandler(cartId, c);
}

function modifyCartItem(cartId, delta){
    fetch(`/Shop/ModifyCartItem?cartId=${cartId}&delta=${delta}`,
    {
        method: 'PUT'
    }).then(r => r.json())
        .then(j => {
            if (j.status == 200) {
                window.location.reload();
            }
            else if(j.status == 422)
            {
                window.alert("Not enough goods in stock");
            }
            else
            {
                console.log(j.message);
                window.alert("Mistake occur, please try again later");
            }
        });
}
function addToCartClick(e){
    e.stopPropagation();
    e.preventDefault();
    const elem = document.querySelector('[data-auth-ua-id]');
    if (!elem){
        window.alert('Please login to submit order')
        return;
    }
    const uaId = elem.getAttribute('data-auth-ua-id');
    const productId = e.target.closest('[data-cart-product-id]').getAttribute('data-cart-product-id');
    console.log(productId, uaId);
    
    fetch('/Shop/AddToCart/', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `productId=${productId}&uaId=${uaId}`
    }).then(r => r.json().then(j => {
        if(j.status == 200)
        {
            window.alert("added to cart");
        }
        else
        {
            window.alert(j.message);
        }
    }));
}

function cartHandler(cartId, choice){
    fetch(`/Shop/CartHandler/?cartId=${cartId}&choice=${choice}`,
        {
            method: 'PUT'
        }).then(r => r.json())
        .then(j => {
            if (j.status == 200) {
                window.alert(j.message)
                window.location.reload();
            }
            else
            {
                console.log(j.status);
                window.alert(j.message);
            }
        });
}