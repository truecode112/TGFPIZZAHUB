"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (formattedData, rawData) {
    console.log(rawData);
    processOrder(formattedData, rawData);
    // data[0] = ''; data[data.length - 1] = '';
    // document.getElementById("hubriseresponse").innerHTML = new;
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    //li.textContent = `${user} says ${message}`;
});

function processOrder(formattedData, rawData) {
    try {
        console.log(typeof formattedData);
        var encoded = btoa(unescape(encodeURIComponent(formattedData)))
        var order = JSON.parse(rawData);
        var expectTime;
        if (order.new_state.expected_time != null) {
            expectTime = new Date(order.new_state.expected_time);
        } else {
            expectTime = new Date();
        }

        var orderCard = `<summary class="card mt-2 flex-row" onClick="viewOrder(event, '${encoded}', '${order.id}', '${order.location_id}', '${expectTime}')" data-order-id="${order.id}"> 
        <div class="ml-3 mr-3 w-10 flex-column d-flex align-items-center justify-content-center">
            <p class="card-title mt-0 mb-0 font-weight-bold" >${expectTime.getHours()}:${expectTime.getMinutes()}</p> 
            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" class="bi bi-bicycle" viewBox="0 0 16 16"> 
                <path d="M4 4.5a.5.5 0 0 1 .5-.5H6a.5.5 0 0 1 0 1v.5h4.14l.386-1.158A.5.5 0 0 1 11 4h1a.5.5 0 0 1 0 1h-.64l-.311.935.807 1.29a3 3 0 1 1-.848.53l-.508-.812-2.076 3.322A.5.5 0 0 1 8 10.5H5.959a3 3 0 1 1-1.815-3.274L5 5.856V5h-.5a.5.5 0 0 1-.5-.5zm1.5 2.443-.508.814c.5.444.85 1.054.967 1.743h1.139L5.5 6.943zM8 9.057 9.598 6.5H6.402L8 9.057zM4.937 9.5a1.997 1.997 0 0 0-.487-.877l-.548.877h1.035zM3.603 8.092A2 2 0 1 0 4.937 10.5H3a.5.5 0 0 1-.424-.765l1.027-1.643zm7.947.53a2 2 0 1 0 .848-.53l1.026 1.643a.5.5 0 1 1-.848.53L11.55 8.623z" /> 
            </svg> 
        </div> 
        <div class=""> 
            <h6 class="card-title mb-0 mt-1">${order.new_state.customer.address_1 == null || typeof order.new_state.customer.address_1 != "string" ? "No address" : order.new_state.customer.address_1} ${order.new_state.customer.address_2 == null ? "" : order.new_state.customer.address_2}</h6> 
            <p class="mt-0 mb-0" style="font-size:13px;">${order.new_state.customer.first_name} ${order.new_state.customer.last_name}</p> 
            <p class="card-text mt-0 mb-1" style="font-size:13px;">${order.new_state.items.length} Product${order.new_state.items.length > 1 ? "s" : ""} (${order.new_state.total})</p> 
            <input type="hidden" name="order_id" value="${order.id}"/> 
        </div> 
    </summary>`;
    }
    catch (err) {
        console.log("err", err);
    }

    document.getElementById("hubriseresponse").innerHTML += orderCard;//new DOMParser().parseFromString(data, 'text/xml').body;
    $(document).ready(function () {
        $('.timepicker').timepicker({
            timeFormat: 'H:mm',
            defaultTime: `${expectTime.getHours()}:${expectTime.getMinutes()}`,
            dynamic: false,
            dropdown: false,
            scrollbar: true
        });
    });
}

connection.on("OldMessages", function (oldMessages) {
    
    try {
        var orderArray = JSON.parse(oldMessages);
        if (orderArray != null && orderArray != undefined) {
            orderArray.forEach(order => {
                processOrder(order.Formated, order.Json);
            });
        }
    }
    catch (err) {
        console.log("err", err);
    }
});

connection.on("ReceiveOrderAccept", function (result) {
    if (result == "OK") {
        $.toast({
            heading: 'Success',
            text: 'Order accepted successfully!',
            showHideTransition: 'fade',
            position: 'top-right',
            icon: 'success'
        });
        closeModal();
    } else {
        console.log(typeof result);

        $.toast({
            heading: 'Error',
            text: JSON.parse(result).message,
            showHideTransition: 'fade',
            position: 'top-right',
            icon: 'error'
        })
    }
});

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");

    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
})


function changeDelivery(action) {
    console.log('changeDelivery', action);
    var modal = $('#orderModal');
    var expect_time = $("#confirmtime").val();
    var expect_date = new Date(expect_time);
    if (action == 'minus') {
        expect_date = new Date(expect_date.getTime() - 10 * 60 * 1000);
    } else {
        expect_date = new Date(expect_date.getTime() + 10 * 60 * 1000);
    }
    $("#confirmtime").val(expect_date);
    modal.find('#delivery_time').text(expect_date.getHours() + ":" + expect_date.getMinutes());
}

function acceptOrder() {
    console.log('acceptOrder');
    var modal = $('#orderModal');
    var order_id = modal.data('orderid');
    var location_id = modal.data('locationid');
    var expect_time = $("#confirmtime").val();
    console.log(expect_time);

    var postData = {
        "confirmed_time": new Date(expect_time).toISOString(),
        "location_id": location_id,
        "order_id": order_id
    };

    connection.invoke('SendData', JSON.stringify(postData)).catch(function (err) {
        return console.error(err.toString());
    });

    /*
    $.ajax({
        type: "PATCH",
        url: `https://api.hubrise.com/v1/location/${location_id}/orders/${order_id}`,
        headers: {
            'Access-Control-Allow-Origin': '*',
            'X-Access-Token': '02117ecb2c2fb78e4da8c33d1d031bd1',
            'X-Http-Method-Override': 'PATCH'
        },
        data: JSON.stringify(postData),
        // data: data,
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            console.log(response)
            $.toast({
                heading: 'Success',
                text: 'Order accepted successfully!',
                showHideTransition: 'fade',
                position: 'top-right',
                icon: 'success'
            })
            closeModal();
        },
        error: function (err) {
            $.toast({
                heading: 'Error',
                text: 'Accept order failed',
                showHideTransition: 'fade',
                position: 'top-right',
                icon: 'error'
            })
        }
    });*/
}

start();