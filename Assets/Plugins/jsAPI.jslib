

mergeInto(LibraryManager.library, {

  startReceivingSystemValues: function() {
    var scripts = document.querySelectorAll('script[src="jlib.js"]');
    if(scripts.length == 0){

      
      var script  = document.createElement("script");
      script.type = "text/javascript";
      script.src  = "system_model.js";
      script.onload = function() {
           setInterval(fetchData, 1);
           console.log("loaded: system_model.js");
      };
      document.head.appendChild(script);
    }
  },

 



  jsGetVariableValue: function(variableName) {
    if (typeof variableStates !== 'undefined') {
      return variableStates[variableName]
    } else {
      console.log("pinValues is not defined");
      return "0";
    }
   
  },



  jsSetVariableValue: function(variableNane, Value){
    if (typeof postPinValue === 'function') {
      // func is defined
      sendVariableToServer(variableNane, Value);
    } else {
      // func is not defined
      console.log("postPinValue is not defined");
    }
    //postPinValue(GPIO_ID, Value);
  },


  output: function(GPIO_ID, Value){
    if (typeof postPinValue === 'function') {
      // func is defined
      postPinValue(GPIO_ID, Value);
    } else {
      // func is not defined
      console.log("postPinValue is not defined");
    }
    // postPinValue(GPIO_ID, Value);
  },

  input: function(GPIO_ID) {
    if (typeof pinValues !== 'undefined') {
      return pinValues[GPIO_ID]
    } else {
      console.log("pinValues is not defined");
      return 0;
    }
    //return pinValues[GPIO_ID];
  },





  
 
});