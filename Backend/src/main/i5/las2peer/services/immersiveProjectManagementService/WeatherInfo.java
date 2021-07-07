package i5.las2peer.services.immersiveProjectManagementService;

public class WeatherInfo {

	private Temperature main;
	private String name;
	
	public Temperature getTemperature() {
		return main;
	}
	public void setMain(Temperature main) {
		this.main = main;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}

	
}