"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (formattedData, rawData) {
    console.log(rawData);
    processOrder(formattedData, rawData);
});

function processOrder(formattedData, rawData, isAccepted) {
    try {
        
        var encoded = btoa(unescape(encodeURIComponent(formattedData)))
        var order = JSON.parse(rawData);

        var expectTime;
        if (order.new_state.expected_time != null) {
            expectTime = new Date(order.new_state.expected_time);
        } else {
            expectTime = new Date();
        }

        var company, type;
        if (order.new_state.service_type_ref != null) {
            company = order.new_state.service_type_ref.split("-")[0].toLowerCase();
            type = order.new_state.service_type_ref.split("-")[1].toLowerCase();
        }

        var orderCard = `<summary class="card mt-2 flex-row border border-3" id="${order.order_id}" onClick="viewOrder(event, '${encoded}', '${order.order_id}', '${order.location_id}', '${expectTime}', '${order.new_state.service_type_ref}', '${order.new_state.service_type}')"> 
            <div class="ms-2 w-10 flex-column d-flex align-items-center justify-content-center">
                <img src="/imgs/${company}.png" width="48px" height="48px" />
            </div>
            <div class="ms-2 me-3 w-10 flex-column d-flex align-items-center justify-content-center">
                <p class="card-title mt-0 mb-0 font-weight-bold" >${expectTime.getHours()}:${expectTime.getMinutes()}</p> 
                <img src="/imgs/${type}.png" width="32px" height="32px" />
            </div> 
            <div class=""> 
                <h6 class="card-title mb-0 mt-1">${order.new_state.customer.address_1 == null || typeof order.new_state.customer.address_1 != "string" ? "No address" : order.new_state.customer.address_1} ${order.new_state.customer.address_2 == null ? "" : order.new_state.customer.address_2}</h6> 
                <p class="mt-0 mb-0" style="font-size:10px;">${order.new_state.customer.first_name} ${order.new_state.customer.last_name}</p> 
                <p class="card-text mt-0 mb-1" style="font-size:10px;">${order.new_state.items.length} Product${order.new_state.items.length > 1 ? "s" : ""} (${order.new_state.total})</p> 
                <input type="hidden" id="order_id" name="order_id" value="${order.id}"/> 
                <input type="hidden" id="${order.order_id}_status" value="${isAccepted}" />
            </div> 
        </summary>`;
    }
    catch (err) {
        console.log("err", err);
    }

    if (isAccepted) {
        document.getElementById("acceptedorders").innerHTML += orderCard;
    } else {
        document.getElementById("pendingorders").innerHTML += orderCard;//new DOMParser().parseFromString(data, 'text/xml').body;
    }
    
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

connection.on("CurrentOrders", function (currentOrders) {
    
    try {
        //console.log(oldMessages);
        document.getElementById("acceptedorders").innerHTML = "";
        document.getElementById("pendingorders").innerHTML = "";
        var orderArray = JSON.parse(currentOrders);
        if (orderArray != null && orderArray != undefined) {
            orderArray.forEach(order => {
                processOrder(order.Formatted, order.Json, order.IsAccepted);
            });
        }
    }
    catch (err) {
        console.log("err", err);
    }
});

connection.on("ChangeOrderStatusOK", function (status, orderId) {
    if (status != "confirmed")
        $("#savingModal").modal("hide");

    if (status == "accepted") {
        $.toast({
            heading: 'Success',
            text: 'Order accepted successfully!',
            showHideTransition: 'fade',
            position: 'top-right',
            icon: 'success'
        });
        $("#" + orderId + "_status").val('true');
        var orderHTML = $("#" + orderId).prop("outerHTML");
        console.log(orderHTML);
        $("#" + orderId).remove();
        document.getElementById("acceptedorders").innerHTML += orderHTML;
    } else if (status == "confirmed") {
        $.toast({
            heading: 'Success',
            text: 'Order confirmed successfully!',
            showHideTransition: 'fade',
            position: 'top-right',
            icon: 'success'
        });
        $("#" + orderId).remove();
    } else {
        $.toast({
            heading: 'Success',
            text: 'Order rejected successfully!',
            showHideTransition: 'fade',
            position: 'top-right',
            icon: 'success'
        });
        $("#" + orderId).remove();
    }
    closeModal();
});

connection.on("ChangeOrderStatusFail", function (error, orderId) {
    $("#savingModal").modal("hide");
    $.toast({
        heading: 'Error',
        text: JSON.parse(error).message,
        showHideTransition: 'fade',
        position: 'top-right',
        icon: 'error'
    })
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

function printOrder() {

    //printElement(document.getElementById("order-modal-body"));
    var order_id = $('#orderId').val();
    connection.invoke('PrintTicket', order_id).catch(function (err) {
        return console.error(err.toString());
    });
}

function changeOrderStatus(status) {
    if (status != "confirmed")
        $("#savingModal").modal("show");

    var modal = $('#orderModal').val();
    var order_id = $('#orderId').val();
    var location_id = $('#orderLocationId').val();
    var expect_time = $("#confirmtime").val();

    var postData = {
        "confirmed_time": new Date(expect_time).toISOString(),
        "location_id": location_id,
        "order_id": order_id,
        "status": status
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