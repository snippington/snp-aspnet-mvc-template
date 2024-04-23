// Helpers
function iget(id){
    return document.getElementById(id);
}
// End Helpers


// UI Tracking
var listMode = 'list';
var showingForm = false;
var showingToast = false;
var showingDetails = false;
var selected = 0;
function toggleDataForm(){
    var offscreenForm = iget('offscreenForm');
    offscreenForm.classList.toggle('offscreen-bar-sm-show');
    //var elem = iget('modalForm');
    if (showingForm){
        //elem.className = 'modal';
        showingForm = false;

    }else{
        //elem.className = 'modal active';
        showingForm = true;
    }
    console.log(offscreenForm.getAttribute('data-form-mode'));
}

function toggleDetailsModal(item){
    var detailsTemplate = '';
    var offscreenBar = document.getElementById('offscreenForm');
    //var elem = iget('detailsModal');
    if (showingDetails){
        //elem.className = 'modal';
        
        offscreenBar.classList.toggle('offscreen-bar-sm-show');
        showingDetails = false;
    }else{
        if(item){
            detailsTemplate = `<div class="card">
                <div class="card-header">
                <div class="card-title modal-title h5">Details</div>
                <p>Name : ${item.name}</p>
                <p class="mt-8"><button class="btn btn-primary" onclick="toggleDetailsModal();">OK</button></p>
            </div>`;
                //var holder = iget('detailsContent');
                //holder.innerHTML = detailsTemplate;
                offscreenBar.classList.toggle('offscreen-bar-sm-show');
                offscreenBar.innerHTML = detailsTemplate;
            }
            //elem.className = 'modal active';
            showingDetails = true;
    }
}

function hidePagination(){
    listMode = 'search';
    iget('paginationHolder').style.display = 'none';
}

function showPagination(){
    listMode = 'list';
    iget('paginationHolder').style.display = 'block';
}

function clearSearch(){
    iget('q').value='';
    gotoPage(1);
    showPagination();
}

function showToast(msgType, msg){
    var elem = iget('toastHolder');
    var toastTemplate = '';
    if (msgType){
        toastTemplate = `<div class="toastr toast-${msgType}">
            <button class="btn btn-clear float-right" onClick="hideToast();"></button>
            ${msg}
        </div>`;
    }
    elem.innerHTML = toastTemplate;
    elem.className = 'toastr show';
    showingToast = true;
}

function hideToast(){
    var elem = iget('toastHolder');
    if (showingToast){
        elem.className = 'toastr hide';
        showingToast = false;
    }
}

function makeListHolderEmpty(){
    var emptyState = `<td colspan="5">
            <div class="text-center empty">
                <progress class="progress" max="100"></progress>
                <span class="badge" data-badge="">loading</span>
            </div>
        </td>`;
        iget('listItemsHolder').innerHTML = emptyState;
}

function gotoPage(pageNumber){
    currentPage = pageNumber;
    renderPagingList();
    makeListHolderEmpty();
    getAllItems();
}

// End UI Tracking

// Request Core
function makeRequest(method, url, data, callback, useToken){
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if(xhr.readyState === XMLHttpRequest.DONE){
            if (xhr.status === 200){
                callback(xhr.responseText);
            }else{
                showToast('error', `Unable to make request. Check your connection.`);
            }

        }
    }
    xhr.open(method, url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    if (useToken){
        xhr.setRequestHeader(
            'RequestVerificationToken',
            document.getElementsByName('__RequestVerificationToken')[0].value
        );
    }
    xhr.send(JSON.stringify(data));
}

function makeFormRequest(method, url, data, callback, useToken){
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function(){
        if (xhr.readyState === XMLHttpRequest.DONE){
            if (xhr.status === 200){
                callback(xhr.responseText);
            }else{
                showToast('error', `Unable to make request. Check your connection.`);
            }
        }
    }
    xhr.open(method, url, true);
    if (useToken){
        xhr.setRequestHeader(
            'RequestVerificationToken',
            document.getElementsByName('__RequestVerificationToken')[0].value
        );
    }
    xhr.send(data);
}
// End Request Core

//Templating
function renderListItem(item){
    var renderedItem = `
        <td>${item.name.substring(0,20)}</td>
            
        <td>
            <button class="btn btn-sm text-light bg-dark" onClick="fillById(${item.sampleProgramId});" id="editBtn${item.sampleProgramId}">Edit</button> |
            <button class="btn btn-sm btn-primary" onClick="getById(${item.sampleProgramId});" id="detailsBtn${item.sampleProgramId}">Details</button> |
            <button class="btn btn-sm text-dark bg-light" onClick="deleteItem(${item.sampleProgramId},'${item.name}');" id="delBtn${item.sampleProgramId}">X</button>
        </td>
    `;
    return renderedItem;
}

var currentPage = 1;
var pageSize = 7;
var totalPageCount = 1;
function renderPagingList(){
    totalPageCount = Math.ceil(listCount / pageSize);
    var pageLimit = 0;
    var startPage = 0;

    //Extra Logic
    if (currentPage >= 2){
        startPage = currentPage - 1;
        if (totalPageCount <= (currentPage + 1)){
            pageLimit = totalPageCount;
        }else{
            pageLimit = currentPage + 1;
        }
    }else{
        startPage = 1;
        if (totalPageCount < 3){
            pageLimit = totalPageCount;
        }else{
            pageLimit = 3;
        }
    }

    var pageButtons = '';
    var activeSwitch = '';
    for (let i = startPage; i <= pageLimit; i++){
        if((i+1) == currentPage){
            activeSwitch = ' active';
        }else{
            activeSwitch = '';
        }
        pageButtons += ` <button class="btn btn-sm" onclick="gotoPage(${i});">${i}</button>`;
    }
    var content = `<div class="column col-12 text-center">${pageButtons}</div>`;
    iget('paginationHolder').innerHTML = content;
}
// End Templating

// Lookups
// End Lookups

// Data Calls
var listCount = 0;

function getCount(){
    makeRequest('GET', '/sampleprogram/count', null, function(response){
        listCount = parseInt(response);
        renderPagingList();
    });
}

function getAllItems() {
    setTimeout(function(){
        var skip = (currentPage - 1) * pageSize;
        var getAllUrl = '/sampleprogram/all?skip=' + skip;
        makeRequest('GET', getAllUrl, null, function(response){
            var items = JSON.parse(response);
            var itemsList = iget('listItemsHolder');
            itemsList.innerHTML = '';
            items.result.forEach(function(item){
            var elem = document.createElement('tr');
            elem.id = 'item' + item.sampleProgramId;
            var template = renderListItem(item);
            elem.innerHTML = template;
            itemsList.appendChild(elem);
            });
        });
    }, 1500);
}

function searchItems(){
    var term = iget('q').value;
    if (term.length>2){
        hidePagination();
        makeRequest('GET', '/sampleprogram/search?term=' + term, null, function(response){
            var items = JSON.parse(response);
            var itemsList = iget('listItemsHolder');
            itemsList.innerHTML = '';
            items.result.forEach(function(item){
                var elem = document.createElement('tr');
                elem.id = 'item' + item.sampleProgramId;
                var template = renderListItem(item);
                elem.innerHTML = template;
                itemsList.appendChild(elem);
            });
        });
    }
    if (term.length == 0){
        gotoPage(1);
        showPagination();
    }
}

function addItem(){
    var formData = new FormData();
    var name = iget('Name').value;
    var isActive = iget('IsActive').value;
    formData.append("Name", name);
    formData.append("IsActive", isActive);
    makeFormRequest('POST', '/sampleprogram/create',
        formData,
        function(response){
            var item = JSON.parse(response);
            toggleDataForm();
            listCount += 1;
            showToast('primary', `Added "${item.value.name}".`);
            renderPagingList();
            if (currentPage == 1)
        {
            // Lookup ref.
        var template = renderListItem(item.value);
        var elem = document.createElement('tr');
        elem.id = 'item' + item.value.sampleProgramId;
        elem.innerHTML = template;
        var itemsList = iget('listItemsHolder');

        if (listCount-1 > pageSize){
            itemsList.removeChild(itemsList.lastChild);
        }
        itemsList.insertBefore(elem, itemsList.firstChild);

        }
        iget("frmData").reset();
        setTimeout(function() {
            hideToast();
        }, 7000);
        }, true);
}

function deleteItem(id, name) {
    makeRequest('POST', '/sampleprogram/delete/' + id, {id:id}, function(response){
        const element = iget('item' + id);
        element.className = 'greyed-out';
        //element.remove();
        listCount -= 1;
        totalPageCount = Math.ceil(listCount / pageSize);
        if (currentPage > totalPageCount){
            currentPage = totalPageCount;
            gotoPage(currentPage);
        }
        iget('editBtn' + id).disabled = true;
        iget('detailsBtn' + id).disabled = true;
        iget('delBtn' + id).disabled = true;
        showToast('error', `Removed "${name}".`);
        renderPagingList();
        setTimeout(function() {
            hideToast();
        }, 7000);
    }, true);
}


function getById(id){
    makeRequest('GET','/sampleprogram/'+id, {id : id}, function(response){
        var item = JSON.parse(response);
        toggleDetailsModal(item);
    }, false);
}

function fillById(id){
    selected = id;
    makeRequest('GET', '/sampleprogram/' + id, {id : id}, function(response){
        var item = JSON.parse(response);
        iget('Name').value = item.name;
        iget('frmData').setAttribute('action', 'sampleprogram/edit/' + item.sampleProgramId);
        iget('offscreenForm').setAttribute('data-form-mode', 'edit');    
        toggleDataForm();
    }, false);
}


function editItem(){
    var formData = new FormData();
        var name = iget('Name').value;
    var isActive = iget('IsActive').value;
    formData.append("SampleProgramId", selected);
    formData.append("Name", name);
    formData.append("IsActive", isActive);
    makeFormRequest('POST', iget('frmData').getAttribute('action'),
        formData,
        function(response) {
            toggleDataForm();
            var item = JSON.parse(response);
            var template = renderListItem(item.result);
            iget('item' + item.result.sampleProgramId).innerHTML = template;
            showToast('primary', `Updated "${iget('Name').value}".`);
        }, true);
}

// End Data Calls

// Event Binding
window.onload = function init(){
    iget('btnAddData').addEventListener('click', function(){
        iget('frmData').setAttribute('action', 'sampleprogram/create');
        iget('offscreenForm').setAttribute('data-form-mode','create');
        iget("frmData").reset();
        toggleDataForm();
    });
    iget('btnRefresh').addEventListener('click', function(){
        gotoPage(1);
    });
    iget('btnAdd').addEventListener('click', function(){
        var elem = iget('offscreenForm');
        var mode = elem.getAttribute('data-form-mode');
        if (mode=='create'){
            addItem();
        }else{
            editItem();
        }
    });
    getCount();
    getAllItems();
    iget('q').focus();
}
// End Event Binding
