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

function getBase64Image(img) {
    var img_src = img.src;
    return img_src.replace(/^data:image\/(png|jpeg);base64,/, "");
}

function eachNode(rootNode, callback) {
    if (!callback) {
        const nodes = [];
        eachNode(rootNode, (node) => {
            nodes.push(node);
        });
        return nodes;
    }

    if (callback(rootNode) === false) {
        return false;
    }

    if (rootNode.hasChildNodes()) {
        for (const node of rootNode.childNodes) {
            if (eachNode(node, callback) === false) {
                return;
            }
        }
    }
}

function printElement(elem) {
    var domClone = elem.cloneNode(true);
    eachNode(domClone, function (node) {
        if (node != undefined && node.style != undefined && node.style.color != undefined)
            node.style.color = "black";
    });

    var opt = {
        margin: 1,
        filename: 'order.pdf',
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'B5', orientation: 'portrait' }
    };
    var img = html2pdf().set(opt).from(domClone).outputImg().then(function (pdfobj) {
        var base64 = getBase64Image(pdfobj);
        $.ajax({
            type: "POST",
            url: 'http://127.0.0.1:9100/htbin/print_base64.py',
            data: { d: base64 },
            success: function (list_printers) {

            },
            error: function (err) {
                console.log(err);
            }
        });
    }).catch(function (err) {
        console.log(err);
    });
    //window.print();
}