function scLogWatcher() {
    this.render(0);
    this.update();

    setTimeout("scLogWatcherObj.ping()", this.pinginterval);
}

scLogWatcher.prototype.pinginterval = 5000;

scLogWatcher.prototype.ping = function (msg) {
    this.update();

    setTimeout("scLogWatcherObj.ping()", this.pinginterval);
};

scLogWatcher.prototype.update = function () {
    new Ajax.Request('/sitecore/shell/Applications/LogErrorViewer/LogWatcher.ashx', {
        onSuccess: function (response) {
            scLogWatcherObj.render(response.responseText);
        }
    });
};

scLogWatcher.prototype.render = function (number, messages) {
    var content = '';
    if (number == 0) {
        content = "<span id='LogErrorWatcher' onmouseout='javascript:return scForm.rollOver(this,event)' class='scToolbutton' onmouseover='javascript:return scForm.rollOver(this,event)' tabindex='0' onkeydown='javascript:scForm.handleKey(this, event, \"logerrorwatcher:menu\", \"32\")' onblur='javascript:return scForm.rollOver(this,event)' onfocus='javascript:return scForm.rollOver(this,event)' onclick='javascript:return scForm.postEvent(this,event,\"logerrorwatcher:menu\")' title='No Errors found so far'><img src='/~/icon/Applications/24x24/bullet_ball_green.png.aspx' width='16' height='16' align='middle' class='' style='margin:0px 1px 0px 1px' alt='No Errors found so far' border='0'></script></span>";
    } else {
        content = "<span id='LogErrorWatcher' onmouseout='javascript:return scForm.rollOver(this,event)' class='scToolbutton' onmouseover='javascript:return scForm.rollOver(this,event)' tabindex='0' onkeydown='javascript:scForm.handleKey(this, event, \"logerrorwatcher:menu\", \"32\")' onblur='javascript:return scForm.rollOver(this,event)' onfocus='javascript:return scForm.rollOver(this,event)' onclick='javascript:return scForm.postEvent(this,event,\"logerrorwatcher:menu\")' title='There some errors (" + number + ") in your log'><img src='/~/icon/Applications/24x24/bullet_ball_red.png.aspx' width='16' height='16' align='middle' class='' style='margin:0px 1px 0px 1px' alt='There some errors (" + number + ") in your log' border='0'>" + number + "</span>";
    }

    $('LogErrorWatcher').replace(content);
}

function scInitializeLogWatcher() {
    scLogWatcherObj = new scLogWatcher();
}

scForm.browser.attachEvent(window, "onload", scInitializeLogWatcher);