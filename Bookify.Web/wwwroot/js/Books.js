$(document).ready(function () {
    //-----------------------------------------------------
    // We Use This Code Only With Metronic Theme
    $('[data-kt-filter="search"]').on('keyup', function () {
        var input = $(this);
        datatable.search(input.val()).draw();
    });
    //-----------------------------------------------------
    datatable = $('#Books').DataTable({
        serverSide: true,
        stateSave: true,
        /* lengthMenu:[5,20,30,40],*/ // For Changing Default Options In The Page Lenght Select List 
        processing: true,
        language: {
            processing: '<div class="d-flex justify-content-center text-primary align-items-center dt-spinner"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div><span class="text-muted ps-2">Loading...</span></div>'
        },
        ajax: {
            url: '/Books/GetBooks',
            type: 'POST',
            data: {
                '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        },
        'drawCallback': function () {
            KTMenu.createInstances();
        },
        order:
            [
                [1, 'asc'] // Sorting With Book Title Ascending
            ],
        columnDefs: [
            {
            targets: [0],
            visible: false,
            searchable: false
            },
            {
                targets: [5],   // Index Of Categories Column
                orderable:false // For Stop Sorting By This Column
            }
        ],
        columns: [
            {
                "data": "id",
                "name": "Id",
                "className": "d-none"
            },
            {
                "name": "Title",
                "render": function (data, type, row) {
                    return `<div class="d-flex align-items-center">
                                <a href="/Books/Details/${row.id}">
                                   <div class="me-3">
                                       <img width="50" height="70" src="${(row.imageThumbnailUrl === null ? '/images/books/no-book.jpg' : row.imageThumbnailUrl)}" alt="${row.title}" class="inline-flex items-center justify-center rounded-full bg-primary text-lg font-bold leading-6 text-primary-text shadow-solid-2 shadow-body-bg [&amp;:nth-child(n+2)]:hidden lg:[&amp;:nth-child(n+2)]:inline-flex h-8 w-8 sm:h-9 sm:w-9">
                                   </div>
                                </a>
                               <div -mt-0.5 text-base">
                                   <span class="text-body-secondary-color">
                                      <a href="/Books/Details/${row.id}" class="text-primary fw-bolder mb-1">
                                        <div>
                                           <span>${row.title}</span>
                                        </div>
                                      </a>
                                     ${row.authorName}
                                   </span>
                               </div>
                            </div>`
                }
            },
            {
                "data": "publisher",
                "name": "Publisher"
            },
            {
                "name": "PublishingDate",
                "render": function (data,type,row) {
                    return moment(row.publishingDate).format('ll')
                }
            },
            {
                "data": "hall",
                "name": "Hall"
            },
            {
                "data": "categories",
                "name": "Categories"
            }
            ,
            {
                "name": "IsAvailableForRental",
                "render": function (data, type, row) {
                    return ` <span class="badge badge-light-${(row.isAvailableForRental ? 'success' : 'warning')} me-auto">
                                         ${(row.isAvailableForRental ? 'Available' : 'Not Available')}
                                </span>` 
                }
            },
            {
                "name": "IsDeleted",
                "render": function (data, type, row) {
                    return ` <span class="badge badge-light-${(row.isDeleted ? 'danger' : 'success')} me-auto  js-status">
                                         ${(row.isDeleted ? 'Deleted' : 'Available')}
                                </span>`
                }
            },
            {
                "className": 'text-end',
                "orderable": false,
                "render": function (data, type, row) {
                    return `<a href="#" class="btn btn-light btn-active-light-primary btn-sm" data-kt-menu-trigger="click" data-kt-menu-placement="bottom-end">
                            Actions
                            <!--begin::Svg Icon | path: icons/duotune/arrows/arr072.svg-->
                            <span class="svg-icon svg-icon-5 m-0">
                                <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                    <path d="M11.4343 12.7344L7.25 8.55005C6.83579 8.13583 6.16421 8.13584 5.75 8.55005C5.33579 8.96426 5.33579 9.63583 5.75 10.05L11.2929 15.5929C11.6834 15.9835 12.3166 15.9835 12.7071 15.5929L18.25 10.05C18.6642 9.63584 18.6642 8.96426 18.25 8.55005C17.8358 8.13584 17.1642 8.13584 16.75 8.55005L12.5657 12.7344C12.2533 13.0468 11.7467 13.0468 11.4343 12.7344Z" fill="currentColor"></path>
                                </svg>
                            </span>
                            <!--end::Svg Icon-->
                        </a>
                                <div class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-800 menu-state-bg-light-primary fw-semibold w-200px py-3" data-kt-menu="true" style="">
                            <!--begin::Menu item-->
                            <div class="menu-item px-3">
                                <a href="/Books/Edit/${row.id}" class="menu-link px-3">
                                    Edit
                                </a>
                            </div>
                            <!--end::Menu item-->
                            <!--begin::Menu item-->
                            <div class="menu-item px-3">
                                        <a href="javascript:;" class="menu-link flex-stack px-3 js-toggle-status" data-url="/Books/ToggleStatus/${row.id}">
                                    Toggle Status
                                </a>
                            </div>
                            <!--end::Menu item-->
                        </div>`;
                }
            }
        ]
    });
});
