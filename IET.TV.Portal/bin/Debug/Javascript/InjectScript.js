var page = require('webpage').create(),
    fs = require('fs'),
    system = require('system')
    url = system.args[1];

page.open(url, function () {
    page.injectJs('JavaScriptToInject.js'); //calling the external JavaScript file
    var title = page.evaluate(function () {
        
        return returnTitle(); //returnTitle received from  external JavaScript file
    });
    fs.write("ReadPhantom.txt", title, 'w'); //writing the return 'Title' in the notepad
    phantom.exit();
}
);