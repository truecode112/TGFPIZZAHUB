// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function viewOrder(e, data, order_id, location_id, expected_time) {
    if (e.target.nodeName == 'INPUT')
        return;
    var expectTime = new Date(expected_time);
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

$('#orderModal').on('show.bs.modal', function (event) {
    var modal = $(this);
    var order_id = modal.data('orderid');
    var location_id = modal.data('locationid');
    var order_data = modal.data('orderdata');
    var expect_time = modal.data('confirmtime');
    modal.find("#confirmtime").val(expect_time);
    var decoded = atob(order_data);
    modal.find('.modal-body').prepend(decoded);
    var expect_date = new Date(expect_time);
    console.log(expect_date);
    modal.find('#delivery_time').text(expect_date.getHours() + ":" + expect_date.getMinutes());
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