
$(document).ready(function () {
    loadExpense();

    $("#expSubmitUpdateBtn").click(function (e) {
        e.preventDefault();

        // Validation check
        if (!$("#expenseForm").valid()) {
            return;
        }


        let ExpenseId = $("#ExpenseId").val();
        let apiurl ="https://localhost:7076/api/expenses";
        let reqType="POST"
        let expense = {
            title: $("#Title").val(),
            amount: $("#Amount").val(),
            categoryId: $("#CategoryId").val(),
            expenseDate: $("#ExpenseDate").val()
        };
        
        if ($("#expSubmitUpdateBtn").text() == "Update Expense") {
            apiurl += "/" + ExpenseId;
            reqType = "PUT";
            expense.expenseId = ExpenseId;
            console.log(apiurl + "," + reqType + "," + expense);
        }

        $.ajax({
            url: apiurl,
            type: reqType,
            contentType:'application/json',
            headers: {
                "Authorization":"Bearer "+token
            },
            data: JSON.stringify(expense),
            success: function (response) {
                alert(response.message);
                $("#expenseForm")[0].reset();

                loadExpense();
            },
            error: function (xhr) {
                console.log(xhr);

                let errors = JSON.parse(xhr.responseText).errors;

                let firstError = Object.values(errors)[0][0];

                alert(firstError);
            }
        });
    });
});


function loadExpense()
{
    $.ajax({
        url: 'https://localhost:7076/api/Reports/GetAllExpensesOfToday',
        type: 'GET',
        headers: {
            "Authorization": "Bearer "+token
        },
        success: function (response) {
            if (response.expenses.length != 0) {
                console.log(response)
                let result = "";
                $(response.expenses).each(function (i, item) {
                    console.log(item);
                    result += `
                    <tr>
                        <td id="td_title_${item.expenseId}">`+ item.title + `</td>
                        <td id="td_amount_${item.expenseId}">`+ item.amount + `</td>
                        <td id="td_catname_${item.expenseId}">`+ item.categoryName + `</td>
                        <td id="td_date_${item.expenseId}">` + item.expenseDate.split('T')[0].split('-').reverse().join('-') + `</td>
                        <td>

                            <button class="btn btn-primary btn-sm" onclick="EditExpense(`+ item.expenseId +`)" >Edit</button>

                            <button class="btn btn-danger btn-sm" onclick="DeleteExpense(`+ item.expenseId +`)">Delete</button>

                         </td>
                    </tr>

                `;
                });
                $("#ExpenseTableData").html(result);
            }
            else {
                $("#ExpenseTableData").html(`
                        <tr>
                            <td colspan="5" class="text-center text-dark">
                                No Expense Entries Found For Today.
                            </td>
                        </tr>
                    `);
            }
        },
        error: function (xhr) {
           
            $("#ExpenseTableData").html(`
                        <tr>
                            <td colspan="5" class="text-center text-dark">
                                Unable to load expenses
                            </td>
                        </tr>
                    `);
        }

    });
}
function EditExpense(id) {
    $("#expSubmitUpdateBtn").text("Update Expense");

    $("#ExpenseId").val(id);
    $("#Title").val($("#td_title_"+id).text());
    $("#Amount").val($("#td_amount_" + id).text());

    let expdate = $("#td_date_" + id).text().split('-').reverse().join('-');
    $("#ExpenseDate").val(expdate);

    $("#CategoryId option").each(function () {
        if ($(this).text().trim() ==
            $("#td_catname_"+id).text().trim()) {

            $(this).prop("selected", true);
        }
    });
    
}


function DeleteExpense(id) {
    var res = confirm("Are you sure, you want to delete this expense ?? ");
    if (res) {
        $.ajax({
            url: 'https://localhost:7076/api/expenses/'+id,
            type: 'DELETE',
            headers: {
                Authorization:'Bearer '+token
            },
            data: { id: id },
            success: function (response) {
                alert(response.message);
                loadExpense();
            },
            error: function (err) {
                alert("Error Occur At Delete Expense !! "+err);
            }
            
        });
    }
}