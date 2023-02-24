// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function viewOrder(e, data, order_id, location_id, expected_time) {
    if (e.target.nodeName == 'INPUT')
        return;
    var expectTime = new Date(expected_time);
    var curTime = $("#t-input-" + order_id).val();
    var hour = parseInt(curTime.split(':')[0]);
    var minute = parseInt(curTime.split(':')[1]);
    expectTime.setHours(hour, minute);
    //console.log($("#t-input-" + order_id).val());
    $("#orderModal").attr('data-confirmtime', expectTime);
    $("#orderModal").attr('data-orderid', order_id);
    $("#orderModal").attr('data-locationid', location_id);
    $("#orderModal").attr('data-orderdata', data);
    $("#orderModal").modal("show");
}

function closeModal() {
    $('#orderModal').modal('hide')
}

function acceptOrder() {
    console.log('acceptOrder');
    var modal = $('#orderModal');
    var order_id = modal.data('orderid');
    var location_id = modal.data('locationid');
    var expect_time = modal.data('confirmtime');

    var postData = {
        "status": "accepted",
        "confirmed_time" : expect_time
    };

    $.ajax({
        type: "PATCH",
        url: `https://api.hubrise.com/v1/location/${location_id}/orders/${order_id}`,
        headers: {
            'Access-Control-Allow-Origin': '*',
            'X-Access-Token': '02117ecb2c2fb78e4da8c33d1d031bd1',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS, PATCH',
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
    });
}

$('#orderModal').on('show.bs.modal', function (event) {
    var modal = $(this);
    var order_id = modal.data('orderid');
    var location_id = modal.data('locationid');
    var order_data = modal.data('orderdata');
    var expect_time = modal.data('confirmtime');
    var decoded = atob(order_data);
    modal.find('.modal-body').prepend(decoded);
    /*var button = $(event.relatedTarget) // Button that triggered the modal
    var recipient = button.data('whatever') // Extract info from data-* attributes
    // If necessary, you could initiate an AJAX request here (and then do the updating in a callback).
    // Update the modal's content. We'll use jQuery here, but you could use a data binding library or other methods instead.
    var modal = $(this)
    modal.find('.modal-title').text('New message to ' + recipient)
    modal.find('.modal-body input').val(recipient)*/
})

$('#orderModal').on('hidden.bs.modal', function (e) {
    var modal = $(this);
    modal.find('.modal-body').empty();
})