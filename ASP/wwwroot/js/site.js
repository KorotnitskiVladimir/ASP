// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const elem = document.getElementById('message');
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
                    'Auth': 'Basic ' + credentials
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
})