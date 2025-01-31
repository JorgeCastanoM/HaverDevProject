function saveDraft() {
    document.getElementById("isDraftInput").value = "true";
    var formData = new FormData(document.querySelector('#formQa'));
    // for (var pair of formData.entries()) {
    //     console.log(pair[0] + ', ' + pair[1]);
    // }
    $.ajax({
        url: '/NcrQa/Create',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                alert(response.message);

                window.location.href = '/NcrQa/Index';
            } else {
                alert('Error saving the draft');
            }
        },
        error: function (xhr, status, error) {
            alert('Error saving the draft: ' + error);
        }
    });
}