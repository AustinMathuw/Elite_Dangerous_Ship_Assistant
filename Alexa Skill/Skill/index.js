/*
EliteD Alexa Skill
Author: Austin Wilson (16)
*/

//Import assests
const https = require('https');
var skillSetup = require('./skillSetup'); //Don't touch
var commandSpeech = require('./speech');
var savedShipInformation;

String.prototype.capitalize = function() {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

//API url
var apiURL = "https://www.edsm.net/api-v1/system?";

//PubHub server information (This is how i send the information to the Raspberry Pi)
var iotCloud = require("pubnub")({
  	ssl           : true,  // - enable TLS Tunneling over TCP 
  	publish_key   : "pub-c-234c4038-44a7-4173-9fd7-e1f6151c56d7", //If you want to host this yourself, this is where your publish_key and subscribe_key will go.
  	subscribe_key : "sub-c-53cc9228-a35d-11e6-a1b1-0619f8945a4f"
});

//var myChannel = "my_device";
var myChannel = Math.floor(Math.random()*90000000) + 10000000;
var myChannelShipInfo = "" + myChannel + "A" //UNCOMMENT if you are hosting this yourself.
var myChannelShipCommands = "" + myChannel + "B" //UNCOMMENT if you are hosting this yourself.

var APP_ID = undefined;

var EliteD = function () {
    skillSetup.call(this, APP_ID);
};

// Extend skillSetup
EliteD.prototype = Object.create(skillSetup.prototype);
EliteD.prototype.constructor = EliteD;

EliteD.prototype.eventHandlers.onSessionStarted = function (sessionStartedRequest, session) {
    console.log("onSessionStarted requestId: " + sessionStartedRequest.requestId
        + ", sessionId: " + session.sessionId);
};

EliteD.prototype.eventHandlers.onLaunch = function (launchRequest, session, response) {
    console.log("onLaunch requestId: " + launchRequest.requestId + ", sessionId: " + session.sessionId);
    handleWelcomeRequest(session, response); //Sends my skill's welcome information
};


/*
Use the following to retrieve the game info:

iotCloud.history({
	channel : myChannelShipInfo,
	callback : function(m){
		//Handle what to do with Info
	},
	count : 1, // 100 is the default
	reverse : false // false is the default
});
*/


EliteD.prototype.intentHandlers = {
	"UnrecognizedIntent": function ( intent, session, response ) {
		unrecognizedSpeech(session, response);
	},

	"WhereAmIIntent": function ( intent, session, response ) {
		iotCloud.history({
			channel : myChannelShipInfo,
			callback : function(m){
			    console.log(m)
			    if(m[0][0]){
			        if(m[0][0].Location){
        				savedShipInformation = m[0];
        				console.log(savedShipInformation);
        				if (savedShipInformation[0].Location.StarSystem){
        					speechOutput = "You are located in " + savedShipInformation[0].Location.StarSystem + ". ";
        					repromptOutput = "What's next?";
    						response.ask(speechOutput+repromptOutput, repromptOutput);
        				} else {
        					speechOutput = "I can't find your location. ";
        					repromptOutput = "What's next?";
    						response.ask(speechOutput+repromptOutput, repromptOutput);
        				}
    				} else {
    					speechOutput = "I can't find your location. ";
    				    repromptOutput = "What's next?";
    				    response.ask(speechOutput+repromptOutput, repromptOutput);
    				}
			    } else {
					speechOutput = "I can't find your location. ";
				    repromptOutput = "What's next?";
				    response.ask(speechOutput+repromptOutput, repromptOutput);
				}
				
			},
			count : 1, // 100 is the default
			reverse : false, // false is the default
			error : function() {
				console.log("error");
				speechOutput = "I can't find your location. ";
				repromptOutput = "What's next?";
				response.ask(speechOutput+repromptOutput, repromptOutput);
			}
		});
		
	},
	
	"GetRecentMessageIntent": function ( intent, session, response ) {
	    console.log(myChannelShipInfo);
		iotCloud.history({
			channel : myChannelShipInfo,
			callback : function(m){
			    savedShipInformation = m[0];
			    console.log(m[0]);
			    if(savedShipInformation[0]){
			        if(savedShipInformation[0].RecieveText.Message){
        				if (savedShipInformation[0].RecieveText){
        					speechOutput = "Here is your latest message from "+savedShipInformation[0].RecieveText.From+": " + savedShipInformation[0].RecieveText.Message.replace(/u0027/g,"'") + " ";
        					repromptOutput = "What's next?";
							response.ask(speechOutput+repromptOutput, repromptOutput);
        				} else {
        					speechOutput = "I can't find your most recent message. ";
        					repromptOutput = "What's next?";
							response.ask(speechOutput+repromptOutput, repromptOutput);
        				}
    				} else {
    					speechOutput = "I can't find your most recent message. ";
				        repromptOutput = "What's next?";
				        response.ask(speechOutput+repromptOutput, repromptOutput);
    				}
			    } else {
					speechOutput = "I can't find your most recent message. ";
				    repromptOutput = "What's next?";
				    response.ask(speechOutput+repromptOutput, repromptOutput);
				}
				
			},
			count : 1, // 100 is the default
			reverse : false, // false is the default
			error : function() {
				console.log("error");
				speechOutput = "I can't find your most recent message. ";
				repromptOutput = "What's next?";
				response.ask(speechOutput+repromptOutput, repromptOutput);
			}
		});
		
	},

	"RankIntent": function ( intent, session, response ) {
		iotCloud.history({
			channel : myChannelShipInfo,
			callback : function(m){
				savedShipInformation = m[0];
				var rankSlot = intent.slots.rank,
					rankValue;

				console.log(rankSlot);
				console.log(rankSlot.value);

				if (rankSlot && rankSlot.value) {
					rankValue = rankSlot.value.toLowerCase().capitalize();
					console.log(rankValue);
					if (savedShipInformation[0]) {
    					if (savedShipInformation[0].Rank.Combat >= 0){
    						if(rankValue == "Combat" || rankValue == "Trade" || rankValue == "Explore" || rankValue == "Cqc" ) {
    							speechOutput = "You are rank " + savedShipInformation[0].Rank[rankValue.capitalize()] + " in " + rankValue.toLowerCase() + ". ";
    							repromptOutput = "What's next?";
							    response.ask(speechOutput+repromptOutput, repromptOutput);
    						} else if(rankValue == "Empire" || rankValue == "Federation" ) {
    							speechOutput = "You are rank " + savedShipInformation[0].Rank[rankValue.capitalize()] + " in the " + rankValue.toLowerCase() + ". ";
    							repromptOutput = "What's next?";
							    response.ask(speechOutput+repromptOutput, repromptOutput);
    						} else {
    							speechOutput = "I can't find your current rank for that area. ";
    							repromptOutput = "What's next?";
							    response.ask(speechOutput+repromptOutput, repromptOutput);
    						}
    					} else {
    						speechOutput = "I can't find your current rank for that area. ";
				            repromptOutput = "What's next?";
				            response.ask(speechOutput+repromptOutput, repromptOutput);
    					}
					} else {
    					speechOutput = "I can't find your current rank for that area. ";
				        repromptOutput = "What's next?";
				        response.ask(speechOutput+repromptOutput, repromptOutput);
    				}
				} else {
					console.log("6");
					speechOutput = "I can't find your current rank for that area. ";
					repromptOutput = "What's next?";
					response.ask(speechOutput+repromptOutput, repromptOutput);
				}
			},
			count : 1, // 100 is the default
			reverse : false, // false is the default
			error : function() {
				console.log("error");
				speechOutput = "I can't find your current rank for that area. ";
				repromptOutput = "What's next?";
				response.ask(speechOutput+repromptOutput, repromptOutput);
			}
		});
		
	},

	"SystemInformationIntent": function (intent, session, response) {
	    
		var systemSlot = intent.slots.system,
			systemValue,
			sysAllegiance,
			sysGovernment,
			sysFaction,
			sysPopulation,
			sysEconomy,
			sysSecurity,
			sysAllegianceSpeech,
			sysGovernmentSpeech,
			sysFactionSpeech,
			sysPopulationSpeech,
			sysEconomySpeech,
			sysSecuritySpeech,
			sysNameSpeech,
			sysInfo,
			sysName,
			sysPermit;

		if (systemSlot && systemSlot.value) {
			systemValue = systemSlot.value.toLowerCase();
			if(systemValue == "this")
			{
			    iotCloud.history({
        			channel : myChannelShipInfo,
        			callback : function(m){
        			    console.log(m)
        			    if(m[0][0]) {
        			        if(m[0][0].Location){
                				savedShipInformation = m[0];
                				console.log(savedShipInformation);
                				if (savedShipInformation[0].Location.StarSystem){
                					systemValue = savedShipInformation[0].Location.StarSystem;
                					console.log(systemValue)
                					systemValue.replace(' ','+');
                        			console.log(systemValue)
                        			
                        			console.log('https://www.edsm.net/api-v1/system?sysname='+systemValue+'&showPermit=1&showInformation=1')
                        
                        			https.get('https://www.edsm.net/api-v1/system?sysname='+systemValue+'&showPermit=1&showInformation=1', (res) => {
                          				console.log('statusCode:', res.statusCode);
                          				console.log('headers:', res.headers);
                        
                          				res.on('data', function (chunk) {
                        					console.log('BODY: ' + chunk); //Handle API return.
                        					sysInfo = JSON.parse(chunk);
                        					console.log(sysInfo);
                        					
                        					if(sysInfo.name){
                        						sysName = sysInfo.name;
                        						
                        						sysPermit = sysInfo.requirePermit;
                        						if(sysInfo.information.allegiance || sysInfo.information.government || sysInfo.information.faction || sysInfo.information.population || sysInfo.information.state || sysInfo.information.security){
                        							speechOutput = "Here is what I have on "+ sysName +". ";
                        							if(sysInfo.information.allegiance){
                        								sysAllegiance = sysInfo.information.allegiance.toLowerCase();
                        								sysAllegianceSpeech = sysName + " has an allegiance to the " + sysAllegiance + ". ";
                        								speechOutput += sysAllegianceSpeech;
                        							}
                        							if(sysInfo.information.government){
                        								sysGovernment = sysInfo.information.government.toLowerCase();
                        								sysGovernmentSpeech = "It's government is a " +sysGovernment+ ". ";
                        								speechOutput += sysGovernmentSpeech;
                        							}
                        							if(sysInfo.information.faction){
                        								sysFaction = sysInfo.information.faction.toLowerCase();
                        								sysFactionSpeech = "It's faction is " +sysFaction+ ". ";
                        								speechOutput += sysFactionSpeech;
                        							}
                        							if(sysInfo.information.population){
                        								sysPopulation = sysInfo.information.population;
                        								sysPopulationSpeech = "It has a population of " +sysPopulation+ ". ";
                        								speechOutput += sysPopulationSpeech;
                        							}
                        							if(sysInfo.information.state){
                        								sysEconomy = sysInfo.information.state.toLowerCase();
                        								sysEconomySpeech = "It's economy is currently in a " + sysEconomy + " state. ";
                        								speechOutput += sysEconomySpeech;
                        							}
                        							if(sysInfo.information.security){
                        								sysSecurity = sysInfo.information.security.toLowerCase();
                        								sysSecuritySpeech = "It has " +sysSecurity+ " security. ";
                        								speechOutput += sysSecuritySpeech;
                        							}
                        							if (sysPermit) {
                        								speechOutput += "This star system also requires a permit. ";
                        							} else {
                        								speechOutput += "This star system also does not require a permit. ";
                        							}
                        							repromptOutput = "What's next?";
                        							speechOutput += repromptOutput;
                        							
                        							response.ask(speechOutput, repromptOutput);
                        						} else {
                        							repromptOutput = " What's next?";
                        							speechOutput = "I don't have any information on " + sysName + "." + repromptOutput;
                        												
                        							response.ask(speechOutput, repromptOutput);
                        						}
                        					} else {
                        						repromptOutput = " What's next?";
                        						speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
                        						
                        						response.ask(speechOutput, repromptOutput);
                        					}
                        				});
                        			}).on('error', (e) => {
                        				console.error(e);
                        				repromptOutput = " What's next?";
                        				speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
                        									
                        				response.ask(speechOutput, repromptOutput);
                        			});
                				}
            				}
        			    } else {
        			        repromptOutput = " What's next?";
            				speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
            									
            				response.ask(speechOutput, repromptOutput);
        			    }
        			},
        			count : 1, // 100 is the default
        			reverse : false, // false is the default
        		});
			    
			} else {
    			systemValue.replace(' ','+');
    			console.log(systemValue)
    			
    			console.log('https://www.edsm.net/api-v1/system?sysname='+systemValue+'&showPermit=1&showInformation=1')
    
    			https.get('https://www.edsm.net/api-v1/system?sysname='+systemValue+'&showPermit=1&showInformation=1', (res) => {
      				console.log('statusCode:', res.statusCode);
      				console.log('headers:', res.headers);
    
      				res.on('data', function (chunk) {
    					console.log('BODY: ' + chunk); //Handle API return.
    					sysInfo = JSON.parse(chunk);
    					console.log(sysInfo);
    					
    					if(sysInfo.name){
    						sysName = sysInfo.name;
    						
    						sysPermit = sysInfo.requirePermit;
    						if(sysInfo.information.allegiance || sysInfo.information.government || sysInfo.information.faction || sysInfo.information.population || sysInfo.information.state || sysInfo.information.security){
    							speechOutput = "Here is what I have on "+ sysName +". ";
    							if(sysInfo.information.allegiance){
    								sysAllegiance = sysInfo.information.allegiance.toLowerCase();
    								sysAllegianceSpeech = sysName + "'s allegiance is to the " + sysAllegiance + ". ";
    								speechOutput += sysAllegianceSpeech;
    							}
    							if(sysInfo.information.government){
    								sysGovernment = sysInfo.information.government.toLowerCase();
    								sysGovernmentSpeech = "It's government is a " +sysGovernment+ ". ";
    								speechOutput += sysGovernmentSpeech;
    							}
    							if(sysInfo.information.faction){
    								sysFaction = sysInfo.information.faction.toLowerCase();
    								sysFactionSpeech = "It's faction is " +sysFaction+ ". ";
    								speechOutput += sysFactionSpeech;
    							}
    							if(sysInfo.information.population){
    								sysPopulation = sysInfo.information.population;
    								sysPopulationSpeech = "It has a population of " +sysPopulation+ ". ";
    								speechOutput += sysPopulationSpeech;
    							}
    							if(sysInfo.information.state){
    								sysEconomy = sysInfo.information.state.toLowerCase();
    								sysEconomySpeech = "It's economy is currently in a " + sysEconomy + " state. ";
    								speechOutput += sysEconomySpeech;
    							}
    							if(sysInfo.information.security){
    								sysSecurity = sysInfo.information.security.toLowerCase();
    								sysSecuritySpeech = "It has " +sysSecurity+ " security. ";
    								speechOutput += sysSecuritySpeech;
    							}
    							if (sysPermit) {
    								speechOutput += "This star system also requires a permit. ";
    							} else {
    								speechOutput += "This star system also does not require a permit. ";
    							}
    							repromptOutput = "What's next?";
    							speechOutput += repromptOutput;
    							
    							response.ask(speechOutput, repromptOutput);
    						} else {
    							repromptOutput = " What's next?";
    							speechOutput = "I don't have any information on " + sysName + "." + repromptOutput;
    												
    							response.ask(speechOutput, repromptOutput);
    						}
    					} else {
    						repromptOutput = " What's next?";
    						speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
    						
    						response.ask(speechOutput, repromptOutput);
    					}
    				});
    			}).on('error', (e) => {
    				console.error(e);
    				repromptOutput = " What's next?";
    				speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
    									
    				response.ask(speechOutput, repromptOutput);
    			});
			}
		} else {
		    repromptOutput = " What's next?";
			speechOutput = "I'm sorry, I could not get data for that system." + repromptOutput;
								
			response.ask(speechOutput, repromptOutput);
		}
	},
	"BoostIntent": function (intent, session, response) {
		var message = {"command":"boost"};
		var speechOutput = commandSpeech.boost[Math.floor(Math.random() * commandSpeech.boost.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"YouThereIntent": function (intent, session, response) {
		var message = {"command":"youThere"};
		var speechOutput = commandSpeech.youThere[Math.floor(Math.random() * commandSpeech.youThere.length)];
		var speechOutput = "I'm here and ready to help.";
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		response.ask(speechOutput + " " + repromptOutput, repromptOutput);
	},
	"BalencePowerIntent": function (intent, session, response) {
		var message = {"command":"balencePower"};
		var speechOutput = commandSpeech.balencePower[Math.floor(Math.random() * commandSpeech.balencePower.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"CancelDockingIntent": function (intent, session, response) {
		var message = {"command":"cancelDocking"};
		var speechOutput = commandSpeech.cancelDocking[Math.floor(Math.random() * commandSpeech.cancelDocking.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DeployChaffIntent": function (intent, session, response) {
		var message = {"command":"deployChaff"};
		var speechOutput = commandSpeech.deployChaff[Math.floor(Math.random() * commandSpeech.deployChaff.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DeployHardpointsIntent": function (intent, session, response) {
		var message = {"command":"deployHardpoints"};
		var speechOutput = commandSpeech.deployHardpoints[Math.floor(Math.random() * commandSpeech.deployHardpoints.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DeployLandingGearIntent": function (intent, session, response) {
		var message = {"command":"deployLandingGear"};
		var speechOutput = commandSpeech.deployLandingGear[Math.floor(Math.random() * commandSpeech.deployLandingGear.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DeployCargoScoopIntent": function (intent, session, response) {
		var message = {"command":"deployCargoScoop"};
		var speechOutput = commandSpeech.deployCargoScoop[Math.floor(Math.random() * commandSpeech.deployCargoScoop.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DeploySRVIntent": function (intent, session, response) { //Add planetary landing check.
		var message = {"command":"deploySRV"};
		var speechOutput = commandSpeech.deploySRV[Math.floor(Math.random() * commandSpeech.deploySRV.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to the game!", "What's next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"ExitFrameShiftDrive": function (intent, session, response) {
		var message = {"command":"exitFramshift"};
		var speechOutput = commandSpeech.exitFrameShift[Math.floor(Math.random() * commandSpeech.exitFrameShift.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"ExitCruiseIntent": function (intent, session, response) {
		var message = {"command":"exitCruise"};
		var speechOutput = commandSpeech.exitCruise[Math.floor(Math.random() * commandSpeech.exitCruise.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PowerToEnginesIntent": function (intent, session, response) {
		var message = {"command":"powerToEngines"};
		var speechOutput = commandSpeech.powerToEngines[Math.floor(Math.random() * commandSpeech.powerToEngines.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PowertoSystemsIntent": function (intent, session, response) {
		var message = {"command":"powerToSystems"};
		var speechOutput = commandSpeech.powerToSystems[Math.floor(Math.random() * commandSpeech.powerToSystems.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PowerToWeaponsIntent": function (intent, session, response) {
		var message = {"command":"powerToWeapons"};
		var speechOutput = commandSpeech.powerToWeapons[Math.floor(Math.random() * commandSpeech.powerToWeapons.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EmergencyStopIntent": function (intent, session, response) {
		var message = {"command":"emergencyStop"};
		var speechOutput = commandSpeech.emergencyStop[Math.floor(Math.random() * commandSpeech.emergencyStop.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EngageFrameShiftDrive": function (intent, session, response) {
		var message = {"command":"engageFrameshift"};
		var speechOutput = commandSpeech.engageFrameShift[Math.floor(Math.random() * commandSpeech.engageFrameShift.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EngageCruiseIntent": function (intent, session, response) {
		var message = {"command":"engageCruise"};
		var speechOutput = commandSpeech.engageCruise[Math.floor(Math.random() * commandSpeech.engageCruise.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"FlightAssistOffIntent": function (intent, session, response) {
		var message = {"command":"fightAssistOff"};
		var speechOutput = commandSpeech.flightAssistOff[Math.floor(Math.random() * commandSpeech.flightAssistOff.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"FlightAssistOnIntent": function (intent, session, response) {
		var message = {"command":"fightAssistOn"};
		var speechOutput = commandSpeech.flightAssistOn[Math.floor(Math.random() * commandSpeech.flightAssistOn.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"TargetEnemyIntent": function (intent, session, response) {
		var message = {"command":"targetEnemy"};
		var speechOutput = commandSpeech.targetEnemy[Math.floor(Math.random() * commandSpeech.targetEnemy.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"ScreenshotIntent": function (intent, session, response) {
		var message = {"command":"screenshot"};
		var speechOutput = commandSpeech.Screenshot[Math.floor(Math.random() * commandSpeech.Screenshot.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"LaunchIntent": function (intent, session, response) {
		var message = {"command":"launch"};
		var speechOutput = commandSpeech.launch[Math.floor(Math.random() * commandSpeech.launch.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"LightsOffIntent": function (intent, session, response) {
		var message = {"command":"lightsOff"};
		var speechOutput = commandSpeech.lightsOff[Math.floor(Math.random() * commandSpeech.lightsOff.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"LightsOnIntent": function (intent, session, response) {
		var message = {"command":"lightsOn"};
		var speechOutput = commandSpeech.lightsOn[Math.floor(Math.random() * commandSpeech.lightsOn.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardOneHundredPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward100"};
		var speechOutput = commandSpeech.enginesForwardOneHundredPercent[Math.floor(Math.random() * commandSpeech.enginesForwardOneHundredPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardNintyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward90"};
		var speechOutput = commandSpeech.enginesForwardNintyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardNintyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardEightyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward80"};
		var speechOutput = commandSpeech.enginesForwardEightyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardEightyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardSeventyFivePercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward75"};
		var speechOutput = commandSpeech.enginesForwardSeventyFivePercent[Math.floor(Math.random() * commandSpeech.enginesForwardSeventyFivePercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardSeventyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward70"};
		var speechOutput = commandSpeech.enginesForwardSeventyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardSeventyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardSixtyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward60"};
		var speechOutput = commandSpeech.enginesForwardSixtyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardSixtyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardFiftyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward50"};
		var speechOutput = commandSpeech.enginesForwardFiftyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardFiftyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardFortyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward40"};
		var speechOutput = commandSpeech.enginesForwardFortyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardFortyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardThirtyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward30"};
		var speechOutput = commandSpeech.enginesForwardThirtyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardThirtyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardTwentyFivePercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward25"};
		var speechOutput = commandSpeech.enginesForwardTwentyFivePercent[Math.floor(Math.random() * commandSpeech.enginesForwardTwentyFivePercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardTwentyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward20"};
		var speechOutput = commandSpeech.enginesForwardTwentyPercent[Math.floor(Math.random() * commandSpeech.enginesForwardTwentyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesForwardTenPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesForward10"};
		var speechOutput = commandSpeech.enginesForwardTenPercent[Math.floor(Math.random() * commandSpeech.enginesForwardTenPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"NextFireGroupIntent": function (intent, session, response) {
		var message = {"command":"nextFireGroup"};
		var speechOutput = commandSpeech.nextFireGroup[Math.floor(Math.random() * commandSpeech.nextFireGroup.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"NextHostileIntent": function (intent, session, response) {
		var message = {"command":"nextHostile"};
		var speechOutput = commandSpeech.nextHostile[Math.floor(Math.random() * commandSpeech.nextHostile.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"NextSystemIntent": function (intent, session, response) {
		var message = {"command":"nextSystem"};
		var speechOutput = commandSpeech.nextSystem[Math.floor(Math.random() * commandSpeech.nextSystem.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"NextShipIntent": function (intent, session, response) {
		var message = {"command":"nextShip"};
		var speechOutput = commandSpeech.nextShip[Math.floor(Math.random() * commandSpeech.nextShip.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PrevoiusFireGroupIntent": function (intent, session, response) {
		var message = {"command":"prevFireGroup"};
		var speechOutput = commandSpeech.prevFireGroup[Math.floor(Math.random() * commandSpeech.prevFireGroup.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PreviousHostileIntent": function (intent, session, response) {
		var message = {"command":"prevHostile"};
		var speechOutput = commandSpeech.prevHostile[Math.floor(Math.random() * commandSpeech.prevHostile.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"PreviousShipIntent": function (intent, session, response) {
		var message = {"command":"prevShip"};
		var speechOutput = commandSpeech.prevShip[Math.floor(Math.random() * commandSpeech.prevShip.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DockingRequestIntent": function (intent, session, response) {
		var message = {"command":"requestDocking"};
		var speechOutput = commandSpeech.dockingRequest[Math.floor(Math.random() * commandSpeech.dockingRequest.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"DiognosticsIntent": function (intent, session, response) { //Add Check here
		
		var speechOutput = commandSpeech.diognostics[Math.floor(Math.random() * commandSpeech.diognostics.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];
		response.ask(speechOutput + " " + repromptOutput, repromptOutput);
	},
	"CenterHeadsetIntent": function (intent, session, response) {
		var message = {"command":"centerHeadset"};
		var speechOutput = commandSpeech.centerHeadset[Math.floor(Math.random() * commandSpeech.centerHeadset.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"RetractHardpointsIntent": function (intent, session, response) {
		var message = {"command":"retractHardpoints"};
		var speechOutput = commandSpeech.retractHardpoints[Math.floor(Math.random() * commandSpeech.retractHardpoints.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"RetractLandingGearIntent": function (intent, session, response) {
		var message = {"command":"retractLandingGear"};
		var speechOutput = commandSpeech.retractLandingGear[Math.floor(Math.random() * commandSpeech.retractLandingGear.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"RetractCargoScoopIntent": function (intent, session, response) {
		var message = {"command":"retractCargoScoop"};
		var speechOutput = commandSpeech.retractCargoScoop[Math.floor(Math.random() * commandSpeech.retractCargoScoop.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesBackwardOneHundredPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesBack100"};
		var speechOutput = commandSpeech.enginesBackwardOneHundredPercent[Math.floor(Math.random() * commandSpeech.enginesBackwardOneHundredPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesBackwardSeventyFivePercentIntent": function (intent, session, response) {
		var message = {"command":"enginesBack75"};
		var speechOutput = commandSpeech.enginesBackwardSeventyFivePercent[Math.floor(Math.random() * commandSpeech.enginesBackwardSeventyFivePercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesBackwardFiftyPercentIntent": function (intent, session, response) {
		var message = {"command":"enginesBack50"};
		var speechOutput = commandSpeech.enginesBackwardFiftyPercent[Math.floor(Math.random() * commandSpeech.enginesBackwardFiftyPercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"EnginesBackwardTwentyFivePercentIntent": function (intent, session, response) {
		var message = {"command":"enginesBack25"};
		var speechOutput = commandSpeech.enginesBackwardTwentyFivePercent[Math.floor(Math.random() * commandSpeech.enginesBackwardTwentyFivePercent.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"SRVRecoveryIntent": function (intent, session, response) {
		var message = {"command":"SRVRecovery"};
		var speechOutput = commandSpeech.SRVRecovery[Math.floor(Math.random() * commandSpeech.SRVRecovery.length)];
		
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	"StopEnginesIntent": function (intent, session, response) {
		var message = {"command":"cutEngines"};
		var speechOutput = commandSpeech.stopEngines[Math.floor(Math.random() * commandSpeech.stopEngines.length)];
		var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];

		iotCloud.publish({ //Publishes the message to my PubHub Device.
			channel   : myChannelShipCommands,
			message   : message,
			callback  : function(e) { 
				console.log( "SUCCESS!", e ); 
				response.ask(speechOutput + " " + repromptOutput, repromptOutput);
				},
			error     : function(e) { 
				response.ask("Failed to connect to your ship!", "What's Next?");
				console.log( "FAILED! RETRY PUBLISH!", e ); }
		});
	},
	
	"GetSessionIDIntent": function (intent, session, response){
		handleGetSessionIDRequest(session, response); //Get session id spoken
	},

	"ThatsAllIntent": function (intent, session, response){
		var speechOutput = commandSpeech.thatsAll[Math.floor(Math.random() * commandSpeech.thatsAll.length)];
		response.tell(speechOutput);
	},

	"ThankYouIntent": function (intent, session, response){
		var speechOutput = commandSpeech.thankYou[Math.floor(Math.random() * commandSpeech.thankYou.length)];
		response.tell(speechOutput);
	},

    "AMAZON.HelpIntent": function (intent, session, response) {
        handleHelpRequest(response); //Run Help
    },
	
	"AMAZON.StopIntent": function (intent, session, response) { //End Program from StopIntent
        var speechOutput = "Goodbye";
		response.tell(speechOutput);
    },

    "AMAZON.CancelIntent": function (intent, session, response) { //End Program from CancelIntent
        var speechOutput = "Goodbye";
		response.tell(speechOutput);
    }
};

function handleWelcomeRequest(session, response) {
	var repromptSpeech = "For more instructions, please say help me.";
    var speechOutput = "Welcome to your onboard ship assistant. "
		+ "Before we begin, please either refer to your alexa app for your session ID, or ask me for it. "
		+ "You will need it to start the application on your computer. "
		+ repromptSpeech + " What would you like me to do?";
	cardTitle = "Welcome to your onboard ship AI.!";
	cardContent = "Session ID = " + myChannel;
    response.askWithCard(speechOutput, repromptSpeech, cardTitle, cardContent);
}

function handleHelpRequest(response) { //Help Function
    var repromptSpeech = "What would you like me to do?";
    var speechOutput = "I can help control your ship in elite dangerous. "
        + "Try giving me a command like, raise landing gear. "
		+ "You can also ask me about star system information. "
		+ "Try, what do you have on Sol. "
        + repromptSpeech;
	var cardContent = speechOutput;
    response.askWithCard(speechOutput, repromptSpeech, "Instuctions for controlling your A.I.", cardContent);
}

function handleNoSlotRequest(response) { //Runs when invalid motion or color is given
	var speechOutput = {
		speech: "I'm sorry, I do not understand your request. Please try again.",
		type: skillSetup.speechOutputType.PLAIN_TEXT
	};
	var repromptSpeech = {
		speech: "What else can I help with?",
		type: skillSetup.speechOutputType.PLAIN_TEXT
	};
	response.ask(speechOutput, repromptSpeech);
	
}

function handleGetSessionIDRequest(session, response) {
    console.log(1);
	var repromptSpeech,
	    speechOutput = {
	        "type": "SSML",
            "speech": '<speak>Here is your session ID: <say-as interpret-as="spell-out">'+myChannel+'</say-as>. Whats next?</speak>'
        };
	console.log(speechOutput);
	repromptSpeech = "When you are connected, you can ask me commands to control your ship";
	console.log(3);
	response.ask(speechOutput, repromptSpeech);
    console.log(4);
    
}

function unrecognizedSpeech(session, response) {
	var speechOutput = commandSpeech.unknownIntent[Math.floor(Math.random() * commandSpeech.unknownIntent.length)];
	var repromptOutput = commandSpeech.whatsNext[Math.floor(Math.random() * commandSpeech.whatsNext.length)];
	response.ask(speechOutput + " " + repromptOutput, repromptOutput);
}

exports.handler = function (event, context) {
    var carControl = new EliteD();
    carControl.execute(event, context);
};
