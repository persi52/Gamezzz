var dataTable;

$(document).ready(function () {
    loadDataTable();
    toastr.options.preventDuplicates = true;
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "destroy": true,
        "ajax": {
            "url": "/games/getalltolist/",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "photoName", 
                "render": function (data) {                    
                    return '<img class="gameOnListImage" alt="elo" src="'+data+'" />';                    
                }, "width": "20%"

            }, 

            { "data": "title", "width": "30%" },           
            { "data": "category", "width": "30%" },
            { "data": "yearOfRelease", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">                       
                           
                            <button class="CustomBtn CustomBtn2" 
                            onclick=AddToFavourites('/Games/AddToFavourites?gameId='+${data})>
                                Add to Favourites
                            </button>                        
                            
                                                    
                            &nbsp;
                            <br/>
                            <a class='btn btn-secondary text-white' style='cursor:pointer; width:70px;'
                                onclick=Delete('/Games/Delete?id='+${data})>
                                Read More
                            </a>
                            <script>renderButton()</script>
                        </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}
function Delete(url) {
    swal({
        title: "Are u sure?",
        text: "Once deleted, ou will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    console.log(data.success);
                    if (data.success) {

                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
function AddToFavourites(url) {
   //trzeba rozwiazac stackowanie sie alertów
    
                
                $.ajax({                 
                     
                
                type: "POST",
                url: url,
                success: function (data) {
                    console.log(data.success);
                    if (data.success) {

                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                       
                        toastr.error(data.message);
                    }
                }
            });
}

    
        
   


