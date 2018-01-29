
            var resourceWait = 3000,
                maxRenderWait = 10000,
                system = require('system'),
                    url = system.args[1];

            var page = require('webpage').create(),
                count = 0,
                forcedRenderTimeout,
                renderTimeout;

            page.viewportSize = { width: 1280, height: 1024 };

            function doRender() {
            //Taking ScreenShot
                page.render('HomePage.png');
                console.log('Screen shot is done');

           //Deleting the screenshot file
          //  var toDelete = 'HomePage.png';         
          //  var fs = require('fs');
           // fs.remove(toDelete);
                phantom.exit();
               
            }


           //Open the webpage and call render function to render the current page
            page.open(url, function (status) {
                if (status !== "success") {
                    console.log('Unable to load url');
                    phantom.exit();
                } else {
                    forcedRenderTimeout = setTimeout(function () {
                        doRender();
                    }, maxRenderWait);
                }


            });
