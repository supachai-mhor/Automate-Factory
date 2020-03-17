
function OnReceiveSearchMachine(machine) {

    if (machine == "") {
        // search result
        var getDIV_AddMachineResult = document.getElementById("AddMachineResult");
        while (getDIV_AddMachineResult.firstChild) {
            getDIV_AddMachineResult.removeChild(getDIV_AddMachineResult.lastChild);
        }
        //search result detail
        var getDIV_AddMachineResultDetail = document.getElementById("AddMachineResultDetail");
        while (getDIV_AddMachineResultDetail.firstChild) {
            getDIV_AddMachineResultDetail.removeChild(getDIV_AddMachineResultDetail.lastChild);
        }

        const divInfo = document.createElement('div');
        divInfo.className = 'user_info';
        divInfo.innerHTML = '<span>  NOT FOUND MACHINE </span>';
        getDIV_AddMachineResult.appendChild(divInfo);
    }
    else {
        var myMachine = JSON.parse(machine);
        if (myMachine != undefined) {

            // search result
            var getDIV_AddMachineResult = document.getElementById("AddMachineResult");
            while (getDIV_AddMachineResult.firstChild) {
                getDIV_AddMachineResult.removeChild(getDIV_AddMachineResult.lastChild);
            }
            const divImage = document.createElement('div');
            divImage.className = 'img_cont';
            divImage.innerHTML = '<img src="' + myMachine.machineImage + '" class="img-thumbnail user_img">';
            const divInfo = document.createElement('div');
            divInfo.className = 'user_info';
            divInfo.innerHTML = '<span>' + myMachine.name + '</span>' +
                '<input type="button" class="btn btn-success" value="ADD" style="right:20px;position:absolute;" onclick="AddRelationWithMachine()" />';

            getDIV_AddMachineResult.appendChild(divImage);
            getDIV_AddMachineResult.appendChild(divInfo);

            //search result detail
            var getDIV_AddMachineResultDetail = document.getElementById("AddMachineResultDetail");
            while (getDIV_AddMachineResultDetail.firstChild) {
                getDIV_AddMachineResultDetail.removeChild(getDIV_AddMachineResultDetail.lastChild);
            }
            const divDetail = document.createElement('div');
            divDetail.innerHTML = '<dl class="card-link row col-sm-12">' +

                '<dt class="col-sm-4">Plant:</dt><dd class="col-sm-8">' + myMachine.plant + '</dd>' +
                '<dt class="col-sm-4">Process:</dt><dd class="col-sm-8">' + myMachine.process + '</dd>' +
                '<dt class="col-sm-4">Line:</dt><dd class="col-sm-8">' + myMachine.line + '</dd>' +
                '<dt class="col-sm-4">Vendor:</dt><dd class="col-sm-8">' + myMachine.vendor + '</dd>' +
                '<dt class="col-sm-4">Supervisor:</dt><dd class="col-sm-8">' + myMachine.supervisor + '</dd>' +
                '<dt class="col-sm-4">Installed Date:</dt><dd class="col-sm-8">' + myMachine.installed_date + '</dd>' +
                '<dt class="col-sm-4">Description:</dt><dd class="col-sm-8">' + myMachine.description + '</dd>' +
                '</dl>';

            getDIV_AddMachineResultDetail.appendChild(divDetail);

        }
    }
   
}
function SearchMachine() {
    if (document.getElementById("SearchRadioAddMachineHashID").checked) {
        connection.invoke("SendSearchMachine", document.getElementById("SearchAddMachine").value,true).catch(function (err) {
        return console.error(err.toString());
        });
    }
    else {
        connection.invoke("SendSearchMachine", document.getElementById("SearchAddMachine").value,false).catch(function (err) {
            return console.error(err.toString());
        });
    }
   
}
function AddRelationWithMachine() {
    if (document.getElementById("SearchRadioAddMachineHashID").checked) {
        connection.invoke("SendAddRelationWithMachine", document.getElementById("SearchAddMachine").value, true).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        connection.invoke("SendAddRelationWithMachine", document.getElementById("SearchAddMachine").value, false).catch(function (err) {
            return console.error(err.toString());
        });
    }

}
function OnReceiveRelationResult(result) {

    if (result == "error") {
        // search result
        var getDIV_AddMachineResult = document.getElementById("AddMachineResult");
        while (getDIV_AddMachineResult.firstChild) {
            getDIV_AddMachineResult.removeChild(getDIV_AddMachineResult.lastChild);
        }
        //search result detail
        var getDIV_AddMachineResultDetail = document.getElementById("AddMachineResultDetail");
        while (getDIV_AddMachineResultDetail.firstChild) {
            getDIV_AddMachineResultDetail.removeChild(getDIV_AddMachineResultDetail.lastChild);
        }

        const divInfo = document.createElement('div');
        divInfo.className = 'user_info';
        divInfo.innerHTML = '<span>  Can not add this machine </span>';
        getDIV_AddMachineResult.appendChild(divInfo);
    }
    else if (result == "has") {
        
        // search result
        var getDIV_AddMachineResult = document.getElementById("AddMachineResult");
        while (getDIV_AddMachineResult.firstChild) {
            getDIV_AddMachineResult.removeChild(getDIV_AddMachineResult.lastChild);
        }
        //search result detail
        var getDIV_AddMachineResultDetail = document.getElementById("AddMachineResultDetail");
        while (getDIV_AddMachineResultDetail.firstChild) {
            getDIV_AddMachineResultDetail.removeChild(getDIV_AddMachineResultDetail.lastChild);
        }

        const divInfo = document.createElement('div');
        divInfo.className = 'user_info';
        divInfo.innerHTML = '<span>  Already has in your Contacts  </span>';
        getDIV_AddMachineResult.appendChild(divInfo);
    }
    else {
        var myMachine = JSON.parse(result);
        if (myMachine != null) {
            // search result
            var getDIV_AddMachineContact = document.getElementById("contactListIdMachine");
            const liData = document.createElement('li');
            liData.innerHTML = '<div class="d-flex bd-highlight">' +
                '<div class="img_cont"><img src="' + myMachine.machineImage + '" class="rounded-circle user_img"></div>' +
                '<div class="user_info"><span>' + myMachine.name + '</span></div>' +
                '</div>';

            getDIV_AddMachineContact.appendChild(liData);
            document.getElementById("SearchAddMachine").value = "";
        }
        
    }
}
