// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function viewOrder(e, data, order_id, location_id, expected_time, service_type_ref, service_type) {
    if (e.target.nodeName == 'INPUT')
        return;
    var expectTime = new Date(expected_time);
    //console.log($("#t-input-" + order_id).val());
    $("#orderData").val(data);
    $("#orderId").val(order_id);
    $("#orderLocationId").val(location_id);
    $("#confirmtime").val(expected_time);
    $("#serviceTypeRef").val(service_type_ref);
    $("#serviceType").val(service_type);
    $("#orderModal").modal("show");
}

function closeModal() {
    $('#orderModal').modal('hide')
}

$('#orderModal').on('show.bs.modal', function (event) {
    var modal = $(this);
    var order_id = $('#orderid').val();
    var location_id = $('#locationid').val();
    var order_data = $("#orderData").val();
    var expect_time = $('#confirmtime').val();
    var decoded = atob(order_data);
    var service_type_ref = $("#serviceTypeRef").val();
    var service_type = $("#serviceType").val();
    var company, type;
    if (service_type_ref != null) {
        company = service_type_ref.split("-")[0].toLowerCase();
        type = service_type_ref.split("-")[1].toLowerCase();
    }

    modal.find('.modal-body').prepend(decoded);
    var expect_date = new Date(expect_time);
    modal.find('#delivery_time').text(expect_date.getHours() + ":" + expect_date.getMinutes());
    modal.find('#service_type').text(service_type);
    modal.find('#service_type_ref_img').attr('src', (type == "delivery" ? "/imgs/delivery.png" : "/imgs/collection.png"));
    //<img src="${type == " delivery" ? " / imgs / delivery.png" : " / imgs / collection.png"}" width = "36px" height = "36px" />
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