$(document).ready(function () {
    GetNcrs();
});

function GetNcrs() {
    $.ajax({
        url: '/' + section + '/GetNcrs',
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            if (response.length === 0) {
                $('#tblBody').html('<tr><td colspan="4">No New NCRS available</td></tr>');
            } else {
                $('#tblBody').empty(); // Clear existing rows
                for (let i = 0; i < response.length; i++) {
                    const ncr = response[i];
                    const $row = $('<tr>');
                    $row.append(`<td>${ncr.ncrNumber}</td>`);
                    $row.append(`<td>${ncr.supplierName}</td>`);//Supplier
                    $row.append(`<td><button class="btn btn-info" onclick="viewDetails('${ncr.ncrId}')"><i class="bi bi-info-square"></i> Details</button></td>`);
                    $row.append(`<td><button class="btn btn-success" onclick="start('${ncr.ncrNumber}')"><i class="bi bi-play-fill"></i> Start</button></td>`);
                    $('#tblBody').append($row);
                }
            }
        },
        error: function () {
            alert('Error fetching data.'); // Provide more specific error message if possible
        }
    });
}

function start(ncrNumber) {
    window.location.href = '/' + section + '/Create?ncrNumber=' + ncrNumber;
}

function viewDetails(ncrId) {
    window.location.href = '/' + 'NcrEng' + '/Details/' + ncrId;
}