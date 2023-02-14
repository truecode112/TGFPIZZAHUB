"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (formattedData, rawData) {
    console.log(rawData);
    document.getElementById("hubriseresponse").innerHTML = formattedData;//new DOMParser().parseFromString(data, 'text/xml').body;
    // data[0] = ''; data[data.length - 1] = '';
    // document.getElementById("hubriseresponse").innerHTML = new;
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    //li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    
}).catch(function (err) {
    return console.error(err.toString());
});