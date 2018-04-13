phase1 = load('phase_236.txt');
phase2 = load('phase_263.txt');
phase3 = load('phase_273.txt');
phase4 = load('phase_283.txt');
phase5 = load('phase_1332018.txt');
phase6 = load('phase_293.txt');
phase7 = load('phase_1232018.txt');
phase8 = load('phase_932018.txt');
dac1 = load('dac_value_236.txt');
dac2 = load('dac_value_263.txt');
dac3 = load('dac_value_273.txt');
dac4 = load('dac_value_283.txt');
dac5 = load('DAC_value_1332018.txt');
dac6 = load('dac_value_293.txt');
dac7 = load('DAC_Value_1232018.txt');
dac8 = load('DAC_Value_932018.txt');

%%
figure;
plot(phase4, 'LineWidth', 2);
hold on
grid on;
%plot(phase2, 'LineWidth', 2);
%plot(phase3, 'LineWidth', 2);
plot(phase5, 'LineWidth', 2); 

figure;
plot(dac4(:,1), dac4(:,2), 'LineWidth', 2);
hold on;
grid on;
plot(dac5(:,1), dac5(:,2), 'LineWidth', 2);
%%
figure;
plot(phase8);
hold on;
stairs(dac8(:,1), dac8(:,2));