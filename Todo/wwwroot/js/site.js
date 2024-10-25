// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//jquery compiles code into javascript, makes it more readable
//jquery uses $
//communicates with server side without refreshing page
//by creating req and resp JS objects to pass data
function deleteToDo(i)
{
    $.ajax({
        url: 'Home/Delete', //name of controller and method
        type: 'POST', //because we are posting data
        data: {
            id: i 
        },
        success:function() { //refreshing window if successful
            window.location.reload();
        }
    });
}

function populateForm(i)
{
    $.ajax({
        url: 'Home/PopulateForm',
        type: 'GET',
        data: {
            id: i
        },
        dataType: 'json',
        success: function(response) {
            $("#ToDo_Name").val(response.name);
            $("#ToDo_Id").val(response.id);
            $("#form-button").val("Update ToDo");
            $("#form-action").attr("action", "/Home/Update");
        }
    })
}
