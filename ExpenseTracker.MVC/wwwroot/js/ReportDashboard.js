
    
const ApiUrl = "https://localhost:7076/api/";


$(document).ready(function () {
    $(".dateRangeBox").hide();

    


    let currentDate = new Date();
    let currentMonth = currentDate.getMonth()+1;
    let currentYear = currentDate.getFullYear();
    getMonthlyExpenseRecord(currentMonth, currentYear);
    getMonthlyTotal(currentMonth, currentYear);
    getMonthlyCategoryWiseExpenseTotal(currentMonth, currentYear);

    $("#month").val(currentMonth);
    $("#year").val(currentYear);
    $("#txt_ExpenseRecords").text("Monthly Expense Records");

        $(".dateTab").click(function () {
            $(".dateRangeBox").show();
            $(".monthRangeBox").hide();
            $(".dateTab").addClass("active-tab");
            $(".monthTab").removeClass("active-tab");

            $("#catChartBox").hide();
            $("#totalCardsBox").hide();

            $("#expenseRecordTable").empty();
            $("#txt_ExpenseRecords").text("Date Wise Expense Records");
        });

        $(".monthTab").click(function () {
            $(".monthRangeBox").show();
            $(".dateRangeBox").hide();
            $(".monthTab").addClass("active-tab");
            $(".dateTab").removeClass("active-tab");

            $("#catChartBox").show();
            $("#totalCardsBox").show();
            getMonthlyExpenseRecord(currentMonth, currentYear);
            $("#txt_ExpenseRecords").text("Monthly Expense Records");
        });


    $("#GenerateMonthlyReport").click(function () {
        
            let month=$("#month").val();
            let year = $("#year").val();
            if (month == "") {
                alert("Please select Month !!");
                return;
            }
            if (year == "") {
                alert("Please select Year !!");
                return;
            }
            getMonthlyExpenseRecord(month, year);
            getMonthlyTotal(month, year);
            getMonthlyCategoryWiseExpenseTotal(month, year);
        });


    $("#GenerateDateRangeReport").click(function () {
        let fromDate = $("#fromDate").val();
        let toDate = $("#toDate").val();
        if (fromDate == "") {
            alert("Please select From Date !!");
            return;
        }
        if (toDate == "") {
            alert("Please select To Date !!");
            return;
        }
        getDateWiseExpenseReport(fromDate, toDate);
    });


    });


function getMonthlyExpenseRecord(month, year) {
    $.ajax({
        url: ApiUrl+'Reports/monthly_expenses',
        type: 'GET',
        headers: {
            "Authorization":"Bearer "+token
        },
        data: {
            month: month,
            year: year
        },
        success: function (response) {

            var result = "";
            if (response.length != 0) {

                var total = 0;
                $(response).each(function (i, item) {
                    total += parseFloat(item.amount);
                    result += `
                    <tr>
                        <td>${(i+1)}</td>
                        <td>${item.title}</td>
                        <td>&#8377; ${item.amount}</td>
                        <td>${item.categoryName}</td>
                        <td>${item.expenseDate.split('T')[0].split('-').reverse().join('-') }</td>
                    </tr>
                `
                });
                result += `
                    <tr>
                        <td><b>Total Expense</b></td>
                        <td colspan="3"><b>&#8377; ${total}</b></td>
                    </tr>
            `
            }
            else {
                result += `
                    <tr>
                        <td colspan="4" class="text-center">Expense Not Found</td>
                    </tr>
                `
            }
            $("#expenseRecordTable").html(result);
        },
        error: function (xhr) {
            

            alert(xhr.responseText);

            //// validation messages from API
            //if (xhr.status == 400) {
            //    console.log(xhr);

            //    let errors = JSON.parse(xhr.responseText).errors;

            //    let firstError = Object.values(errors)[0][0];

            //    alert(firstError);
            //}
            var result = `
                <tr>
                    <td colspan="4" class=text-center>
                        Unable to Fetch Expenses
                    </td>
                </tr>
            `;

            $("#expenseRecordTable").html(result);
        }
    });
}
function getMonthlyTotal(month, year)
{
    $.ajax({
        url: 'https://localhost:7076/api/Reports/monthly_total',
        type: 'GET',
        headers: {
            "Authorization": "Bearer " + token
        },
        data: {
            month: month,
            year: year
        },
        success: function (response) {

            if (response.total > 0) {
                $("#month_total").html("&#8377; " + response.total);
            }
            else {
                $("#month_total").text("");
            }

        },
        error: function (xhr) {
            //console.log(xhr.responseText);
            $("#month_total").text("");
        }
    });

}
function getMonthlyCategoryWiseExpenseTotal(month, year) {
    $.ajax({
        url: ApiUrl +'Reports/CategoryWiseExpenseTotal',
        type: 'GET',
        headers: {
            "Authorization": "Bearer " + token
        },
        data: {
            month: month,
            year: year
        },
        success: function (response) {
            drawPieChart(response);
            var result = "";
            var maxCategory = response[0];
            if (response.length != 0) {
                $(response).each(function (i, item) {

                    if (item.totalAmount > maxCategory.totalAmount) {
                        maxCategory = item;
                    }

                    result += `
                    <tr>
                        <td>${item.categoryName}</td>
                        <td>&#8377; ${item.totalAmount}</td>
                    </tr>
                `
                });

                $("#maxCategoryName").text(maxCategory.categoryName);
            }
            else {
                result += `
                    <tr>
                        <td colspan="2" class="text-center">Not Found</td>
                    </tr>
                `
                $("#maxCategoryName").text("");
            }
            $("#categoryWiseExpense").html(result);
        },
        error: function (xhr) {
            //console.log(xhr.responseText);

            $("#expenseChart").hide();
            $("#maxCategoryName").text("");
            var result = "";
            result += `
                   <tr>
                        <td colspan="2" class="text-center">Unable To Fetch Data</td>
                    </tr>
                `
            $("#categoryWiseExpense").html(result);
        }
    });

}


function getDateWiseExpenseReport(fromDate, toDate) {

    $.ajax({
        url: ApiUrl +'Reports/date_range',
        type: 'GET',
        headers: {
            "Authorization":"Bearer "+token
        },
        data: { StartDate: fromDate, EndDate: toDate },
        success: function (response) {

            var result = "";
            if (response.length != 0) {

                var total = 0;
                $(response).each(function (i, item) {
                    total += parseFloat(item.amount);
                    result += `
                    <tr>
                        <td>${(i + 1)}</td>
                        <td>${item.title}</td>
                        <td>&#8377; ${item.amount}</td>
                        <td>${item.categoryName}</td>
                        <td>${item.expenseDate.split('T')[0].split('-').reverse().join('-')}</td>
                    </tr>
                `
                });
                result += `
                    <tr>
                        <td><b>Total Expense</b></td>
                        <td colspan="3"><b>&#8377; ${total}</b></td>
                    </tr>
            `
            }
            else {
                result += `
                    <tr>
                        <td colspan="4" class="text-center">Expense Not Found</td>
                    </tr>
                `
            }
            $("#expenseRecordTable").html(result);
        },
        error: function (xhr) {


            //console.log(xhr);

            // validation messages from API
            if (xhr.status == 400) {
                alert(xhr.responseText);
            }
            var result = `
                <tr>
                    <td colspan="4" class=text-center>
                        Unable to Fetch Expenses
                    </td>
                </tr>
            `;

            $("#expenseRecordTable").html(result);
        }
    });
}


let expenseChart;
function drawPieChart(response) {
    let Labels = [];
    let Data = [];
    $(response).each(function (i, item) {
        Labels.push(item.categoryName);
        Data.push(item.totalAmount);
    });
    // pie chart code
    const ctx = document.getElementById('expenseChart');
    if (expenseChart) {
        expenseChart.destroy();
    }
    expenseChart= new Chart(ctx, {

        type: 'pie',

        data: {

            labels: Labels,

            datasets: [{
                data: Data
            }]
        },
        options: {
            plugins: {
                legend: {
                    labels: {
                        color: 'white'
                    }
                }
            }
        }

    });
}