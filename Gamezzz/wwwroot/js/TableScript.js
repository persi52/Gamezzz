var dataTable;


$(document).ready(function () {
    
    loadDataTable();
    toastr.options.preventDuplicates = true;
    
   
        /*$('.CustomBtn').click(function () {

            $.get("/Games/IsFavourite", function (data) {
                console.log(data);

            });

            this.className = "btn btn-secondary text-white";
        });*/

   
});

function loadDataTable() {

    dataTable = $('#ProfileDTLoad').DataTable({
        "destroy": true,
        "ajax": {
            "url": "/games/getfavourites",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "photoName",
                "render": function (data) {
                    return '<img class="gameOnListImage" alt="elo" src="' + data + '" />';
                }, "width": "20%"
            },

            { "data": "title", "width": "30%" },
            { //Nie updatuje bazy
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">                       
                           
                            <button class="btn-danger" 
                            onclick=RemoveFromFavourites('/Games/RemoveFromFavourites?gameId='+${data})>  
                                
                                Remove From Favourites         
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

    dataTable = $('#DT_load').DataTable({
        "destroy": true,
        "ajax": {
            "url": "/games/getalltolist",
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
                           
                            <button class="CustomBtn" 
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
function RemoveFromFavourites(url) {
    swal({
        title: "Are u sure you want to remove game from favourites?",        
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
                        location.reload();
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

function renderClass(data) {

    var isFavourite = true;

    ($.get("/Games/IsFavourite?gameId=" + data, dataType = 'boolean', function (result) {
        if (result == false) isFavourite = false;
        else isFavourite = true;
    }));

    // console.log(isFavourite);

    if (isFavourite == true)
        return 'CustomBtn';
    else return 'btn btn-secondary text-white';

}
   
        
   


