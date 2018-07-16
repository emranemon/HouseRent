
	function initMap()
	{
        var location = area(); // "East West University, Dhaka";
		
    axios.get('https://maps.googleapis.com/maps/api/geocode/json',{
      params:{
        address:location,
        key:'AIzaSyCAUw6hI-oxoDEa1_e_kPJbFj2zDZt3bNs'
      }
    })
    .then(function(response){
      // Log full response
      console.log(response);
      // Geometry
      var lt = response.data.results[0].geometry.location.lat;
      var ln = response.data.results[0].geometry.location.lng;
	  
	  var myLatLng = {lat: lt, lng: ln};

       var map = new google.maps.Map(document.getElementById('map'), {
          zoom: 15,
          center: myLatLng
        });

       var marker = new google.maps.Marker({
          position: myLatLng,
          map: map,
          title: 'Pinned the Location!'
        });
    })
    .catch(function(error){
      console.log(error);
    });
	}
	

  
