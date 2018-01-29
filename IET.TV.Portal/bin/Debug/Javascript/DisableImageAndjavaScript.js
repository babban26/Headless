
    var resourceWait = 3000,
        maxRenderWait = 10000,
        system = require('system'),
         url = system.args[1];
    var page = require('webpage').create(),
        count = 0,
        forcedRenderTimeout,
        renderTimeout;

    page.viewportSize = { width: 1280, height: 1024 };
   //page.settings.webSecurityEnabled = false;   //to enable/disable WebSecurity
    page.settings.javascriptEnabled = true;      //to enable/disable JavaScript
    page.settings.loadImages = false;            //to enable/disable loading Images


    function doRender() {
        page.render('HomePage_withoutImageOrJavaScript.png'); // Capturing the Current webpage
        
    }

    page.open(url, function (status) {
        if (status !== "success") {
            console.log('Unable to load url');
            phantom.exit();
        } else {
            forcedRenderTimeout = setTimeout(function () {
               doRender();
            }, maxRenderWait);
            //phantom.kill();
            phantom.exit();
        }
        
    });

