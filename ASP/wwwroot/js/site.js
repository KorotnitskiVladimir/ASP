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
        if (name.length == 0  || images == null || price <= 0 || stock < 0)
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