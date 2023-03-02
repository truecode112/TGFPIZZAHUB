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
    var acceptStatus = $("#" + order_id + "_status").val();
    $("#isAccepted").val(acceptStatus);
    $("#savingModal").hide();
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
    var is_accepted = $("#isAccepted").val();
    var company, type;
    if (service_type_ref != null) {
        company = service_type_ref.split("-")[0].toLowerCase();
        type = service_type_ref.split("-")[1].toLowerCase();
    }

    modal.find('.order-detail-body').prepend(decoded);
    var expect_date = new Date(expect_time);
    modal.find('#delivery_time').text(expect_date.getHours() + ":" + expect_date.getMinutes());
    modal.find('#service_type').text(service_type);
    modal.find('#service_type_ref_img').attr('src', (type == "delivery" ? "/imgs/delivery.png" : "/imgs/collection.png"));
    if (is_accepted == "true") {
        modal.find('#closeButton').show();
        modal.find('#printButton').show();
        modal.find('#confirmButton').show();
        modal.find('#rejectButton').hide();
        modal.find('#acceptButton').hide();
        modal.find('#time_spin').hide();
    } else {
        modal.find('#closeButton').hide();
        modal.find('#confirmButton').hide();
        modal.find('#printButton').hide();
        modal.find('#rejectButton').show();
        modal.find('#acceptButton').show();
        modal.find('#time_spin').show();
    }
})

$('#orderModal').on('hidden.bs.modal', function (e) {
    var modal = $(this);
    modal.find('.order-detail-body').empty();
})

function printElement(elem) {
    var domClone = elem.cloneNode(true);

    var $printSection = document.getElementById("printSection");

    if (!$printSection) {
        var $printSection = document.createElement("div");
        $printSection.id = "printSection";
        $printSection.style = "width:400px";
        document.body.appendChild($printSection);
    }

    $printSection.innerHTML = "";
    $printSection.appendChild(domClone);
    window.print();
}