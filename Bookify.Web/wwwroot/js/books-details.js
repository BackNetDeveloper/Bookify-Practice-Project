function OnAddCopySuccess(row)
{
    showSuccessMessage();
    $('#Modal').modal('hide');
    $('tbody').prepend(row);
    KTMenu.createInstances();
    
    var Count = $('#CopiesCount');
    var newCount = parseInt(Count.text()) + 1;
    Count.text(newCount);

    $('.js-alert').addClass('d-none');
    $('table').removeClass('d-none');
}
function OnEditCopySuccess(row)
{
    showSuccessMessage();
    $('#Modal').modal('hide');
    $(updatedRow).replaceWith(row);
    KTMenu.createInstances();
}