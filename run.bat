@echo off
:: CD to this script's directory
cd /d "%~dp0"

call venv\Scripts\activate.bat
python main.py