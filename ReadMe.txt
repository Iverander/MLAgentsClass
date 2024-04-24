Installation:
py -help
exit()
py -m venv venv //create 
venv\Scripts\activate //enter the vitual environment
py -m install pip --upgrade pip
pip install mlagents
pip install torch torchvision torchaudio //install torch
pip install onnx

Commonly used CMD commands:
mlagents-learn //start learning
mlagents-learn --force //override learning instance
mlagents-learn --resume //resume learning instance
mlagents-learn --run-id=nameOfInstance (ex: Test2) //change learning instance

pip install "software" //installs something from the pip library