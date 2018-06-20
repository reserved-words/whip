﻿class Player {
    
    constructor() {
        this.playerControls = new PlayerControls();
        this.currentTrack = new CurrentTrack();
        this.currentPlaylist = new CurrentPlaylist();
        this.playerControls.disableControls(true);

        var self = this;

        document.getElementById("controls").onended = function () {
            self.getNextTrack();
        }
        document.getElementById("controls").onseeking = function () {
            self.skipToPercentage();
        }
        $(".play").click(function () {
            self.resume();
        });

        $(".pause").click(function () {
            self.pause();
        });

        $(".next").click(function () {
            self.getNextTrack();
        });

        $(".previous").click(function () {
            self.getPreviousTrack();
        });
    }

    play(secondsPlayed) {
        this.playerControls.play();
        UTIL.post("/Player/Play", null, { secondsPlayed });
    }

    resume() {
        this.playerControls.play();
        UTIL.post("/Player/Resume");
    }

    pause() {
        this.playerControls.pause();
        UTIL.post("/Player/Pause", null, { secondsPlayed: this.playerControls.secondsPlayed() });
    }

    skipToPercentage() {
        var percentage = this.playerControls.skipToPercentage();
        if (percentage === 0)
            return;
        UTIL.post("/Player/SkipToPercentage", null, { percentage });
    }

    stop(secondsPlayed) {
        UTIL.post("/Player/Stop", null, { secondsPlayed });
        this.currentTrack.clearTrackData();
        this.playerControls.stop();
    }

    clearTrack() {
        this.currentTrack.clearTrackData();
        this.playerControls.updateTrack(null);
    }
    
    updateTrack(secondsPlayed, data) {
        this.currentTrack.updateTrackData(data);
        this.playerControls.updateTrack(data);
        this.play(secondsPlayed);
    }

    getNextTrack() {
        var self = this;
        var secondsPlayed = self.playerControls.secondsPlayed();
        self.clearTrack();
        self.currentPlaylist.getNextTrack(function (data) {
            if (!data.Url) {
                self.stop(secondsPlayed);
            }
            else {
                self.updateTrack(secondsPlayed, data);
            }
        });
    }

    getPreviousTrack() {
        var self = this;
        var secondsPlayed = self.playerControls.secondsPlayed();
        self.clearTrack();
        self.currentPlaylist.getPreviousTrack(function (data) {
            self.updateTrack(secondsPlayed, data);
        });
    }

    updatePlaylist(url) {
        var self = this;
        var secondsPlayed = self.playerControls.secondsPlayed();
        self.clearTrack();
        self.currentPlaylist.update(url, function (data) {
            self.updateTrack(secondsPlayed, data);
        });
    }
}